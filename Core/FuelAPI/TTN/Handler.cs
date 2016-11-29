using BaseEntities;
using CoreDM;
using FuelAPI.Config;
using FuelAPI.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace FuelAPI.TTN
{
    public class Handler
    {
        FuelConfig _config;
        public Handler(FuelConfig config)
        {
            _config = config;
        }

        List<string> _files = new List<string>();
        CoreDM.CoreEntities _db = new CoreDM.CoreEntities();
        public void GetFileList()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://10.1.101.41/TTN/");
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential("NBTTN", "vB3T7j");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                _files.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();
            response.Close();
        }
        private void DeleteFile(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://10.1.101.41/TTN/" + fileName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential("NBTTN", "vB3T7j");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }
        public void DownloadFiles()
        {
            if (_files.Count == 0) return;

            Dictionary<string, SysDictionary> _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);

            XmlDocument ttnFile = new XmlDocument();
            foreach (string fileName in _files)
            {
                if (!fileName.EndsWith(".xml"))
                {
                    DeleteFile(fileName);
                    continue;
                }
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://10.1.101.41/TTN/" + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("NBTTN", "vB3T7j");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    ttnFile.Load(responseStream);
                }
                response.Close();
                foreach (XmlNode doc in ttnFile.DocumentElement.SelectNodes("Документы/Документ"))
                {
                    try
                    {
                        TTN.Document ttn = new TTN.Document(doc);
                        IList<FlOrder> orders = _db.FlOrders.Include("Items").Where(p => p.DocDate.Equals(ttn.DocDate) && p.Auto.RegNum.StartsWith(ttn.RegNum) //&& p.State.Code.Equals("1")
                            && p.TankFarm.ShortName.Equals(ttn.PlaceName)).OrderBy(p => p.Order).ToList();
                        long stateCanceledID = _states["2"].ID;
                        var _order = orders.Select(p => new { Order = p, Count = p.Items.Count(i => i.State.ID != stateCanceledID && ((i.WaybillNum == ttn.DocNumber) || (!i.WaybillNum.HasValue))) }).Where(p => p.Count >= ttn.Sections.Count).OrderBy(p => p.Order.Order).FirstOrDefault();
                        if (_order == null)
                        {
                            throw new Exception("План не найден");
                        }
                        else
                        {
                            FlOrder order = _order.Order;
                            foreach (SectionData section in ttn.Sections)
                            {
                                FlOrderItem item = order.Items.FirstOrDefault(p => p.Volume == section.Volume && p.State.ID != stateCanceledID);
                                item.WaybillDate = ttn.DocDate;
                                item.WaybillNum = ttn.DocNumber;
                                item.VolumeFact = section.Volume;
                                item.Density = section.Density;
                                item.Temperature = section.Temperature;
                                item.ReceiveDate = DateTime.Now;
                                item.QPassportNum = section.PassNumber;
                                item.QPassportDate = section.PassDate;
                                item.State = _states["3"];
                            }
                            _db.SaveChanges();
                        }
                        // ttn.CreateDocument(_config.Paths.OutPath + fileName);
                    }
                    catch (Exception e)
                    {
                        var errNode = ttnFile.CreateElement("ErrorMessage");
                        errNode.InnerText = e.Message;
                        ttnFile.DocumentElement.AppendChild(errNode);
                        ttnFile.Save(_config.Paths.ErrorPath + fileName);
                        Console.WriteLine(e.Message);
                    }
                }
                DeleteFile(fileName);
            }
        }
    }
}
