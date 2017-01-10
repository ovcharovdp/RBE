using BaseEntities;
using CoreDM;
using FuelAPI.Operations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TankBalance.Loader
{
    public class PlanHandler
    {
        CoreEntities _db;
        Dictionary<string, SysDictionary> _states;
        long _canceledStateID;
        StreamWriter _logFile;
        public PlanHandler(CoreEntities db, StreamWriter file)
        {
            _db = db;
            _logFile = file;
            _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
            _canceledStateID = _states["2"].ID;
        }

        public bool Handle(FlStation station, DateTime date, string[] data)
        {
            decimal volume = decimal.Parse(data[26], CultureInfo.InvariantCulture);
            decimal density = decimal.Parse(data[27], CultureInfo.InvariantCulture);
            int weight = (int)decimal.Parse(data[29], CultureInfo.InvariantCulture);
            Regex rgx = new Regex(@"\d+");
            Match m = rgx.Match(data[17]);
            int waybill = int.Parse(m.Value);
            rgx = new Regex(@"\d{2}");
            m = rgx.Match(data[18]);
            if (!m.Success)
            {
                _logFile.WriteLine(string.Format("Не определен грузоотправитель: {0},{1},{2},{3},{4},{5}", data[18].Trim(), date, volume, waybill, station.Name, data[10].Trim()));
                return false;
            }
            string tankFarm = m.Value;
            string regNum = AutoOperations.GetFormatedRegNum(data[24].Trim());

            FlOrderItem q = _db.FlOrderItems.Include("State").Include("Order").FirstOrDefault(p => p.Order.TankFarm.ShortName.StartsWith(tankFarm)
                && p.WaybillNum == waybill
                && p.VolumeFact == volume
                && p.State.ID != _canceledStateID);
            if (q != null)
            {
                // предусмотреть несовпадение АЗС и изменение плана
                q.State = _states["4"];
                q.ReceiveDate = date;
                if (q.Station.ID != station.ID)
                {
                    _logFile.WriteLine(String.Format("АЗС по плану и факту не совпадают: {0},{1},{2},{3},{4}", regNum, date, volume, waybill, station.Name));
                }
            }
            else
            {
                TRNAuto auto = _db.TRNAutos.FirstOrDefault(p => p.RegNum.StartsWith(regNum) || p.RegNumExt.StartsWith(regNum));
                if (auto == null)
                {
                    _logFile.WriteLine(string.Format("Авто не найдено: {0},{1},{2},{3},{4},{5}", regNum, date, volume, waybill, station.Name, data[10].Trim()));
                    return false;
                }
                DateTime startDate = date.Date.AddDays(-1);
                DateTime endDate = date.Date;
                var d = _db.FlOrderItems.Include("State").Include("Order").Where(p => p.Station.ID == station.ID
                    && p.Order.Auto.ID == auto.ID
                    && p.Order.TankFarm.ShortName.StartsWith(tankFarm)
                    && p.Order.DocDate >= startDate
                    && p.Order.DocDate <= endDate
                    && p.Volume > volume - 50 && p.Volume < volume + 50
                    && p.State.ID != _canceledStateID
                    && p.WaybillNum == null).ToList();
                if (d.Count > 1)
                {
                    d = d.Where(p => p.Volume > volume - 3 && p.Volume < volume + 3).ToList();
                }
                if (d.Count == 1)
                {
                    q = d[0];
                    switch (q.State.Code)
                    {
                        // "Погружен"
                        case "3":
                            q.State = _states["4"];
                            q.ReceiveDate = date;
                            break;
                        // "Запланирован"
                        case "1":
                            q.State = _states["4"];
                            q.ReceiveDate = date;
                            q.WaybillNum = waybill;
                            q.Weight = weight;
                            q.VolumeFact = volume;
                            q.WaybillDate = date.Date;
                            q.Density = density;
                            break;
                        default: break;
                    }
                }
                else
                {
                    _logFile.WriteLine(String.Format("{0} соответствий для слива: {1},{2},{3},{4},{5},{6}", d.Count, regNum, date, volume, waybill, station.Name, data[10].Trim()));
                    return false;
                }
            }
            _db.SaveChanges();
            OrderOperations.ChangeState(q.Order);
            return true;
        }
    }
}
