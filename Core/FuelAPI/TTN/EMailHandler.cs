using BaseEntities;
using CoreDM;
using FuelAPI.Config;
using FuelAPI.Operations;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace FuelAPI.TTN
{
    public class EMailHandler
    {
        FuelConfig _config;
        public EMailHandler(FuelConfig config)
        {
            _config = config;
        }
        CoreDM.CoreEntities _db = new CoreDM.CoreEntities();
        public void DownloadFiles()
        {
            Pop3Client objClient = new Pop3Client();
            objClient.Connect(_config.Email.Host, _config.Email.Port, false);
            objClient.Authenticate(_config.Email.User, _config.Email.Password);
            int comCount = objClient.GetMessageCount();
            if (comCount == 0)
                return;

            Dictionary<string, SysDictionary> _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
            for (int idx = 1; idx <= comCount; idx++)
            {
                Message dd = objClient.GetMessage(idx);

                if (true/*dd.Headers.From.Address.Equals("dispetcher_enpu@www.tcnp.ru")*/)
                {

                    foreach (var a in dd.FindAllAttachments())
                    {

                        if (!a.FileName.EndsWith(".xml"))
                            continue;

                        using (Stream st = new MemoryStream())
                        {

                            a.Save(st);
                            st.Position = 0;
                            XmlDocument ttnFile = new XmlDocument();
                            ttnFile.Load(st);
                            XmlNode root = ttnFile.DocumentElement;
                            long stateCanceledID = _states["2"].ID;
                            foreach (XmlNode doc in root.SelectNodes("Документы/Документ"))
                            {
                                try
                                {
                                    string error = string.Empty;
                                    TTN.Document ttn = new TTN.Document(doc);
                                    byte sectionNum = ttn.Sections[0].SectionNum;
                                    FlOrderItem item = _db.FlOrderItems.Include("Order").Where(p => p.Order.Auto.RegNum.StartsWith(ttn.RegNum)
                                        && p.Order.DocDate.Equals(ttn.DocDate) && p.Order.TankFarm.ShortName.Equals(ttn.PlaceName)
                                        && p.SectionNum == sectionNum
                                        && (p.WaybillNum == ttn.DocNumber || !p.WaybillNum.HasValue)
                                        && p.State.ID != stateCanceledID).OrderBy(p => p.Order.Order).FirstOrDefault();
                                    if (item == null)
                                    {
                                        throw new Exception("План не найден");
                                    }
                                    foreach (SectionData section in ttn.Sections)
                                    {
                                        item.WaybillDate = ttn.DocDate;
                                        item.WaybillNum = ttn.DocNumber;
                                        item.VolumeFact = section.Volume;
                                        item.Density = section.Density;
                                        item.Temperature = section.Temperature;
                                        item.ReceiveDate = DateTime.Now;
                                        item.QPassportNum = section.PassNumber;
                                        item.QPassportDate = section.PassDate;
                                        item.State = _states["3"];
                                        item.Weight = section.Weight;
                                        if (section.PassDensity > 0)
                                        {
                                            item.QDensity = section.PassDensity;
                                        }
                                        section.AllowExport = true;
                                    }
                                    _db.SaveChanges();
                                    if (item.Station.Number > 0)
                                    {
                                        if (!item.Station.Code.HasValue)
                                        {
                                            error = "Для АЗС " + item.Station.Name + " не задан идентификатор АСУТП";
                                        }
                                        else
                                        {
                                            ttn.StationID = item.Station.Code.GetValueOrDefault().ToString();
                                            ttn.CustomerName = item.Station.Organization.FullName;
                                            ttn.CustomerCode = item.Station.Organization.ID.ToString();
                                            ttn.CreateDocument(_config.Paths.OutPath + a.FileName);
                                        }
                                    }
                                    if (OrderOperations.ChangeState(item.Order))
                                        _db.SaveChanges();

                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        throw new Exception(error);
                                    }
                                }
                                catch (Exception e)
                                {
                                    var errNode = ttnFile.CreateElement("ErrorMessage");
                                    errNode.InnerText = e.Message;
                                    root.AppendChild(errNode);
                                    ttnFile.Save(_config.Paths.ErrorPath + a.FileName);
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                }
                objClient.DeleteMessage(idx);
            }
            objClient.Disconnect();
        }
    }
}
