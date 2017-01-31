using BaseEntities;
using CoreAPI.Operations;
using CoreDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuelAPI.Operations
{
    public class OrderOperations : BaseDBOperations
    {
        public static long New(CoreEntities DB, FlOrder element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
                DB.FlOrders.Add(element);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании заказа.", e);
            }
            return element.ID;
        }
        public static IQueryable<SysDictionary> GetStates(CoreEntities db)
        {
            return from g in db.ObjGroups.Where(p => p.Code.Equals("A991679A-9FD5-49CC-B69D-83E0DF8610CA"))
                   join og in db.ObjGroupObjects on g.ID equals og.GroupID
                   join d in db.SysDictionaries on og.ObjectID equals d.ID
                   select d;
        }
        public static long AddItem(CoreEntities DB, FlOrderItem element)
        {
            try
            {
                if (element.ID == 0)
                    element.ID = GetNextID(DB);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании заказа.", e);
            }
            return element.ID;
        }
        public static bool ChangeState(FlOrder order)
        {
            try
            {
                SysDictionary state = order.Items.Where(p => !p.State.Code.Equals("2")).Select(p => p.State).Distinct().SingleOrDefault();
                order.State = state;
                if (state.Code.Equals("3"))
                    order.FillDateFact = DateTime.Now;
                List<FlOrderItem> i = order.Items.Where(p => p.State.Equals(state)).ToList();
                order.Volume = i.Sum(p => p.VolumeFact);
                order.Weight = i.Sum(p => p.Weight).GetValueOrDefault(0);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static FlOrderItem SetStation(CoreEntities db, FlOrderItem item, long stationID)
        {
            bool needChangeState = false;
            FlStation station = db.FlStations.Find(stationID);
            if (station == null)
                throw new ArgumentException("АЗС не найдена (ID:" + stationID.ToString() + ")");
            var order = item.Order;
            if (item.Station.ID != stationID)
            {
                var _states = GetStates(db).ToDictionary(p => p.Code, p => p);
                FlOrderItem newItem = new FlOrderItem()
                {
                    SectionNum = item.SectionNum,
                    TankNum = item.TankNum,
                    Volume = item.Volume,
                    Station = station,
                    Product = item.Product,
                    State = _states[item.State.Code],
                    Customer = station.Organization,
                    Density = item.Density,
                    QPassportDate = item.QPassportDate,
                    QPassportNum = item.QPassportNum,
                    QDensity = item.QDensity,
                    ReceiveDate = DateTime.Now,
                    Temperature = item.Temperature,
                    VolumeFact = item.VolumeFact,
                    Weight = item.Weight,
                    WaybillNum = item.WaybillNum,
                    WaybillDate = item.WaybillDate,
                    IsChanged = true
                };
                AddItem(db, newItem);
                if (item.WaybillNum == null)
                {
                    Regex rgx = new Regex(@"\d{2}");
                    Match m = rgx.Match(order.TankFarm.ShortName);
                    string tankFarm = m.Value;
                    var endDate = order.DocDate.AddDays(2);
                    DateTime startDate = order.DocDate;
                    string autoNum = order.Auto.RegNum;
                    var d = db.FlFacts.Where(p => p.Station.ID == stationID
                        && p.FactDate >= startDate
                        && p.FactDate < endDate
                        && autoNum.StartsWith(p.RegNum)
                        && p.TankFarmCode.StartsWith(tankFarm)
                        && p.Volume > item.Volume - 50 && p.Volume < item.Volume + 50).ToList();

                    if (d.Count == 1)
                    {
                        newItem.State = _states["4"];
                        newItem.ReceiveDate = d[0].FactDate;
                        newItem.WaybillNum = d[0].WaybillNum;
                        newItem.Weight = d[0].Weight;
                        newItem.VolumeFact = d[0].Volume;
                        newItem.WaybillDate = d[0].FactDate.Date;
                        newItem.Density = d[0].Density;
                        needChangeState = true;
                        db.FlFacts.Remove(d[0]);
                    }
                }
                item.Order.Items.Add(newItem);
                item.ReceiveDate = DateTime.Now;
                item.State = _states["2"];
                item.IsChanged = true;
            }
            if (needChangeState)
                ChangeState(order);
            db.SaveChanges();
            return item;
        }
    }
}
