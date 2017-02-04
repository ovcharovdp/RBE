using BaseEntities;
using CoreAPI.Operations;
using CoreDM;
using FuelAPI.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuelAPI.Fact
{
    public class FactHandler
    {
        CoreEntities _db;
        Dictionary<string, SysDictionary> _orderStates;
        Dictionary<string, SysDictionary> _factStates;
        long _canceledStateID;
        public FactHandler(CoreEntities db)
        {
            _db = db;
            _orderStates = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
            _canceledStateID = _orderStates["2"].ID;

            _factStates = SysDictionaryOperations.GetItems(_db, "53B28CB3-3415-469D-9664-9CC2C51E21D2").ToDictionary(p => p.Code, p => p);
        }
        public void Handle(FlFact fact)
        {
            if (fact.ProductCode == 40)
                return;

            DateTime startDate = fact.FactDate.Date.AddDays(-3);
            FlOrderItem q = _db.FlOrderItems.Include("State").Include("Order").FirstOrDefault(p => p.Order.DocDate >= startDate
                && p.WaybillNum == fact.WaybillNum
                && p.VolumeFact == fact.Volume
                && p.State.ID != _canceledStateID);
            if (q != null)
            {
                q.ReceiveDate = fact.FactDate;
                // предусмотреть несовпадение АЗС и изменение плана
                if (q.Station.ID != fact.Station.ID)
                {
                    FlOrderItem newItem = new FlOrderItem()
                    {
                        SectionNum = q.SectionNum,
                        TankNum = fact.TankNum,
                        Volume = q.Volume,
                        Station = fact.Station,
                        Product = q.Product,
                        State = _orderStates["4"],
                        Customer = fact.Station.Organization,
                        Density = fact.Density,
                        QPassportDate = q.QPassportDate,
                        QPassportNum = q.QPassportNum,
                        QDensity = q.QDensity,
                        ReceiveDate = fact.FactDate,
                        Temperature = q.Temperature,
                        VolumeFact = fact.Volume,
                        Weight = fact.Weight,
                        WaybillNum = q.WaybillNum,
                        WaybillDate = q.WaybillDate,
                        IsChanged = true
                    };
                    OrderOperations.AddItem(_db, newItem);
                    q.Order.Items.Add(newItem);
                    q.State = _orderStates["2"];
                    q.IsChanged = true;
                }
                else
                {
                    q.TankNum = fact.TankNum;
                    q.State = _orderStates["4"];
                }
            }
            else
            {
                string regNum = AutoOperations.GetFormatedRegNum(fact.RegNum);
                TRNAuto auto = _db.TRNAutos.FirstOrDefault(p => p.RegNum.StartsWith(regNum) || p.RegNumExt.StartsWith(regNum));
                if (auto == null)
                {
                    FactOperations.Resolve(_db, fact);
                    fact.State = _factStates["02"];
                    _db.SaveChanges();
                    return;
                }
                Regex rgx = new Regex(@"\d{2}");
                Match m = rgx.Match(fact.TankFarmCode);
                if (!m.Success)
                {
                    FactOperations.Resolve(_db, fact);
                    fact.State = _factStates["01"];
                    _db.SaveChanges();
                    return;
                }
                string tankFarm = m.Value;
                startDate = fact.FactDate.Date.AddDays(-1);
                DateTime endDate = fact.FactDate.Date;
                var d = _db.FlOrderItems.Include("State").Include("Order").Where(p => p.Station.ID == fact.Station.ID
                    && p.Order.Auto.ID == auto.ID
                    && p.Order.TankFarm.ShortName.StartsWith(tankFarm)
                    && p.Order.DocDate >= startDate
                    && p.Order.DocDate <= endDate
                    && p.Volume > fact.Volume - 50 && p.Volume < fact.Volume + 50
                    && p.State.ID != _canceledStateID
                    && p.WaybillNum == null).ToList();
                if (d.Count > 1)
                {
                    q = d.OrderBy(p => Math.Abs(p.Volume - fact.Volume)).FirstOrDefault();
                }
                else
                {
                    if (d.Count == 1) q = d[0];
                }
                if (q != null)
                {
                    switch (q.State.Code)
                    {
                        // "Погружен"
                        case "3":
                            q.State = _orderStates["4"];
                            q.TankNum = fact.TankNum;
                            q.ReceiveDate = fact.FactDate;
                            break;
                        // "Запланирован"
                        case "1":
                            q.State = _orderStates["4"];
                            q.TankNum = fact.TankNum;
                            q.ReceiveDate = fact.FactDate;
                            q.WaybillNum = fact.WaybillNum;
                            q.Weight = fact.Weight;
                            q.VolumeFact = fact.Volume;
                            q.WaybillDate = fact.FactDate.Date;
                            q.Density = fact.Density;
                            break;
                        default: break;
                    }
                }
                else
                {
                    if (!JoinedHandle(fact))
                    {
                        FactOperations.Resolve(_db, fact);
                        fact.State = _factStates["03"];
                        _db.SaveChanges();
                        SplitHandle(fact);
                        return;
                    }
                }
            }
            fact.State = _factStates["00"];
            _db.SaveChanges();
            if (q != null)
                OrderOperations.ChangeState(q.Order);
        }
        public bool JoinedHandle(FlFact fact)
        {
            // смотрим в целом по рейсу
            var q = _db.FlOrders.Where(p => p.Items.Where(i => i.WaybillNum == fact.WaybillNum && i.State.ID == 202).Sum(i => i.VolumeFact) == fact.Volume)
                .SelectMany(p => p.Items.Where(i => i.WaybillNum == fact.WaybillNum && i.State.ID == 202)).ToList();
            if (q.Count == 0)
            {
                // смотрим с фильтром по АЗС
                q = _db.FlOrders.Where(p => p.Items.Where(i => i.WaybillNum == fact.WaybillNum && i.State.ID == 202 && fact.Station.ID == i.Station.ID).Sum(i => i.VolumeFact) == fact.Volume)
                .SelectMany(p => p.Items.Where(i => i.WaybillNum == fact.WaybillNum && i.State.ID == 202 && fact.Station.ID == i.Station.ID)).ToList();
            }
            foreach (var i in q)
            {
                var item = i;
                if (item.Station.ID != fact.Station.ID)
                {
                    // изменяем план
                    item = OrderOperations.SetStation(_db, item, fact.Station.ID);
                }
                // фиксируем слив
                item.ReceiveDate = fact.FactDate;
                item.TankNum = fact.TankNum;
                item.State = _orderStates["4"];
            }
            if (q.Count > 0)
            {
                _db.SaveChanges();
                OrderOperations.ChangeState(q[0].Order);
                return true;
            }
            return false;
        }
        public bool SplitHandle(FlOrderItem item)
        {
            var q = _db.FlFacts.Where(p => p.Station.ID == item.Station.ID && p.WaybillNum == item.WaybillNum && p.FactDate > item.Order.DocDate).ToList();
            if (item.VolumeFact == q.Sum(p => p.Volume))
            {
                item.State = _orderStates["4"];
                item.ReceiveDate = q.Min(p => p.FactDate);
                OrderOperations.ChangeState(item.Order);
                foreach (var f in q)
                {
                    f.State = _factStates["00"];
                }
                return true;
            }
            return false;
        }
        public void SplitHandle(FlFact fact)
        {
            DateTime startDate = fact.FactDate.AddDays(-3);
            long stateID = _orderStates["3"].ID;
            var d = _db.FlOrderItems.Where(p => p.Order.DocDate > startDate && p.State.ID == stateID && p.Station.ID == fact.Station.ID
                && p.VolumeFact == _db.FlFacts.Where(f => f.Station.ID == p.Station.ID && f.WaybillNum == p.WaybillNum && f.FactDate > startDate).Sum(f => f.Volume));

            var q = d.ToList();
            if (q.Count > 0)
            {
                SplitHandle(q[0]);
                _db.SaveChanges();
            }
        }
    }
}
