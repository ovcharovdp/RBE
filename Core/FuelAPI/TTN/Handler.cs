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
        string _currentPath;
        public Handler(FuelConfig config)
        {
            _config = config;
        }

        List<string> _files = new List<string>();
        CoreDM.CoreEntities _db = new CoreDM.CoreEntities();
        private FtpWebResponse FtpAction(string method, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_currentPath + fileName);
            request.Method = method;
            request.Credentials = new NetworkCredential(_config.FtpSource.User, _config.FtpSource.Password);

            return (FtpWebResponse)request.GetResponse();
        }
        private void GetFileList()
        {
            _files.Clear();
            FtpWebResponse response = FtpAction(WebRequestMethods.Ftp.ListDirectory, string.Empty);
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
            FtpWebResponse response = FtpAction(WebRequestMethods.Ftp.DeleteFile, fileName);
            response.Close();
        }
        private void DownloadFiles()
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
                FtpWebResponse response = FtpAction(WebRequestMethods.Ftp.DownloadFile, fileName);
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
                            var q = from o in order.Items.Where(p => p.State.ID != stateCanceledID)
                                    group o by o.Station into stationGroup
                                    select stationGroup;
                            foreach (var station in q)
                            {
                                foreach (FlOrderItem item in station)
                                {
                                    SectionData section = ttn.Sections.FirstOrDefault(p => p.Volume == item.Volume);
                                    if (section == null)
                                    {
                                         section = ttn.Sections.FirstOrDefault(p => p.SectionNum == item.SectionNum);
                                    }
                                    if (section != null)
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
                                        section.AllowExport = true;
                                    }
                                }
                                if (!ttn.Sections.Any(p => p.AllowExport))
                                    continue;

                                _db.SaveChanges();
                                if (!station.Key.Code.HasValue)
                                    throw new Exception("Для АЗС " + station.Key.Name + " не задан идентификатор АСУТП");

                                ttn.StationID = station.Key.Code.GetValueOrDefault().ToString();
                                ttn.CustomerName = station.Key.Organization.FullName;
                                ttn.CustomerCode = station.Key.Organization.ID.ToString();
                                ttn.CreateDocument(_config.Paths.OutPath + fileName.Replace(".xml", ttn.Sections.Count.ToString() + ".xml"));
                                ttn.Sections.RemoveAll(p => p.AllowExport);
                            }
                            if (ttn.Sections.Count > 0)
                            {
                                throw new Exception("Не все секции распределены по плану.");
                            }
                        }
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
        public void Handle()
        {
            for (int i = 0; i < _config.FtpSource.Folders.Count; i++)
            {
                _currentPath = _config.FtpSource.Folders[i].Path;
                GetFileList();
                DownloadFiles();
            }
        }
    }
}
