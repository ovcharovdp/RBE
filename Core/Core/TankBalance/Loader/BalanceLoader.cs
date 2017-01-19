using BaseEntities;
using CoreAPI.Const;
using CoreDM;
using FuelAPI.Fact;
using FuelAPI.Operations;
using OpenPop.Mime;
using OpenPop.Pop3;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TankBalance.Config;

namespace TankBalance.Loader
{
    public class BalanceLoader
    {
        BalanceConfig _config;
        CoreEntities _db;
        FactHandler _factHandler;
        Dictionary<string, SysDictionary> _productList;
        SysDictionary _tankStateActive;
        StreamWriter _logFile;
        public BalanceLoader(BalanceConfig config, StreamWriter file)
        {
            _logFile = file;
            _config = config;
            _db = new CoreEntities();
            _factHandler = new FactHandler(_db);
        }
        private void Init()
        {
            IConstLoader l = new GroupIDLoader(_db);
            long groupID = l.Load("73A6CA9F-4630-43C7-A37B-CF18535809E3");
            var m = from og in _db.ObjGroupObjects.Where(p => p.GroupID == groupID)
                    join d in _db.SysDictionaries on og.ObjectID equals d.ID
                    select d;
            _productList = m.ToDictionary(p => p.Code, p => p);

            _tankStateActive = _db.SysDictionaries.Find(61655);
        }
        void LinkPlan(FlStation station, DateTime date, string[] data)
        {
            FlFact fact = new FlFact()
            {
                Station = station,
                FactDate = date,
                Volume = decimal.Parse(data[26], CultureInfo.InvariantCulture),
                Density = decimal.Parse(data[27], CultureInfo.InvariantCulture),
                Weight = (int)decimal.Parse(data[29], CultureInfo.InvariantCulture),
                TankNum = byte.Parse(data[11]),
                TankFarmCode = data[18].Trim(),
                RegNum = AutoOperations.GetFormatedRegNum(data[24]),
                ProductCode = byte.Parse(data[10])
            };
            Regex rgx = new Regex(@"\d+");
            Match m = rgx.Match(data[17]);
            fact.WaybillNum = int.Parse(m.Value);
            _factHandler.Handle(fact);
        }
        private void LoadBalance(FlStation station, string fileName)
        {
            List<TankData> tanks = new List<TankData>();
            using (StreamReader file = new StreamReader(@".\Out\" + fileName, Encoding.GetEncoding(1251)))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] data = line.Split(';');
                    DateTime balanceDate = DateTime.Parse(data[7] + " " + data[8]);
                    byte operationCode = byte.Parse(data[9]);

                    if (operationCode != 0 && operationCode != 1)
                        continue;

                    byte productCode = byte.Parse(data[10]);
                    byte tankNum = byte.Parse(data[11]);
                    int volume = (int)decimal.Parse(data[12], CultureInfo.InvariantCulture);
                    TankData tank = tanks.FirstOrDefault(p => p.Num == tankNum);
                    if (operationCode == 0)
                    {
                        if (tank == null)
                        {
                            tanks.Add(new TankData() { Num = tankNum, ProductCode = productCode, StartVolume = volume, Volume = volume, BalanceDate = balanceDate, InputVolume = 0 });
                        }
                        else
                        {
                            tank.Volume = volume;
                            tank.BalanceDate = balanceDate;
                        }
                    }
                    if (operationCode == 1 && tank != null)
                    {
                        tank.InputVolume += volume;
                        this.LinkPlan(station, balanceDate, data);
                    }
                }
                file.Close();
            }
            foreach (TankData tank in tanks)
            {
                if (tank.BalanceDate > DateTime.Now)
                {
                    _logFile.WriteLine(fileName + ": дата остатка больше текущей");
                    continue;
                }
                FlStationTank sTank = station.Tanks.FirstOrDefault(p => p.Num == tank.Num);
                if (sTank == null)
                {
                    SysDictionary product;
                    if (tank.ProductCode == 93)
                    {
                        product = _productList["92"];
                    }
                    else
                    {
                        if (!_productList.ContainsKey(tank.ProductCode.ToString()))
                        {
                            _logFile.WriteLine(fileName + ": продукта с кодом " + tank.ProductCode.ToString() + " нет");
                            continue;
                        }
                        else
                        {
                            product = _productList[tank.ProductCode.ToString()];
                        }
                    }
                    sTank = new FlStationTank()
                    {
                        Num = tank.Num,
                        Product = product,
                        ProductCode = tank.ProductCode,
                        State = _tankStateActive,
                        DaySell = tank.StartVolume - tank.Volume + tank.InputVolume,
                        SellDays = 1,
                        Balance = tank.Volume,
                        BalanceDate = tank.BalanceDate
                    };
                    FuelAPI.Operations.StationOperations.AddTank(_db, sTank);
                    station.Tanks.Add(sTank);
                }
                else
                {
                    // если дата в базе более актуальная
                    if (sTank.BalanceDate < tank.BalanceDate)
                    {
                        byte days = sTank.SellDays > 6 ? (byte)6 : sTank.SellDays;
                        sTank.DaySell = (sTank.DaySell * days + tank.StartVolume - tank.Volume + tank.InputVolume) / ++days;
                        sTank.SellDays = days;
                        sTank.Balance = tank.Volume;
                        sTank.BalanceDate = tank.BalanceDate;
                    }
                }
                if (sTank.DaySell > 0)
                    sTank.DeadDate = sTank.BalanceDate.AddDays((sTank.Balance - sTank.DeadVolume) / (double)sTank.DaySell);
            }
            _db.SaveChanges();
        }
        public void Load()
        {
            Init();
            Pop3Client objClient = new Pop3Client();
            objClient.Connect(_config.Email.Host, _config.Email.Port, false);
            try
            {
                objClient.Authenticate(_config.Email.User, _config.Email.Password);
                int comCount = objClient.GetMessageCount();
                if (comCount == 0)
                    return;
                for (int idx = 1; idx <= comCount; idx++)
                {
                    Message dd = objClient.GetMessage(idx);
                    foreach (var a in dd.FindAllAttachments())
                    {
                        if (!a.FileName.EndsWith(".ZIP"))
                            continue;
                        using (Stream st = new MemoryStream())
                        {

                            a.Save(st);
                            st.Position = 0;
                            List<string> files;
                            using (var tmp = new SevenZipExtractor(st))
                            {
                                files = tmp.ArchiveFileData.Where(p => p.FileName.EndsWith("STR")).Select(p => p.FileName).ToList();
                                tmp.ExtractFiles(@".\Out\", files.ToArray());
                            };
                            foreach (string file in files)
                            {
                                FileInfo fileInf = new FileInfo(@".\Out\" + file);
                                try
                                {
                                    int stationCode = int.Parse(fileInf.Name.Remove(5));
                                    FlStation station = _db.FlStations.Include("Tanks").SingleOrDefault(p => p.Code == stationCode);
                                    if (station == null)
                                    {
                                        throw new Exception("АЗС не определено");
                                    }
                                    LoadBalance(station, file);
                                    fileInf.Delete();
                                }
                                catch (Exception e)
                                {
                                    _logFile.WriteLine(file + ":" + e.Message);
                                }
                            }
                        }
                    }
                    objClient.DeleteMessage(idx);
                }
            }
            finally
            {
                objClient.Disconnect();
            }
        }
    }
}
