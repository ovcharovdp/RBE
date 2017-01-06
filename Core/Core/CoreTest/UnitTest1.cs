using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using CoreDM;
using System;

namespace CoreTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RegNumTest()
        {
            CoreEntities db = new CoreEntities();
            //  var ddd = new FlOrderItem() { Volume = 0 };
            long[] states = { 202, 200 };
            DateTime ddd = DateTime.Today.AddDays(-2);
            var q =
            //    db.FlStationTanks.Select(p => new
            //{
            //    Tank = p,
            //    Volume = (double?)db.FlOrderItems.Where(i => i.Station.Equals(p.Station) && i.TankNum == p.Num && i.Order.DocDate > ddd
            //        && states.Contains(i.State.Code)).Sum(i => i.Volume)
            //});
            from t in db.FlStationTanks
                //let plan = db.FlOrderItems
                //where plan.Station.ID == t.Station.ID
                //// from i in db.FlStations//.FlOrderItems on t.Station.ID equals i.Station.ID //into gg
                //// from i in gg.DefaultIfEmpty()
                ////  where i.ID== t.Station.ID 
            select new
            {
                Tank = t,
                Volume = (from i in db.FlOrderItems
                          where i.Station.ID == t.Station.ID
                          && i.TankNum == t.Num
                          && i.Order.DocDate > ddd
                          && states.Contains(i.State.ID)
                          select i.Volume).DefaultIfEmpty(0).Sum()
            };
            //int a = 3;
            //int b = 4;
            //double c = a / b;
            //string num = "О 343 РО 116";
            ////У555ЕР116
            ////У 555 ЕР 116
            //num = num.Replace(" ", "");
            //Regex rgx = new Regex(@"[А-Я]\d{3}[А-Я]{2}");
            //Match m = rgx.Match(num);
            //Regex rgxRegion = new Regex(@"\d{2,3}$");
            //m = rgxRegion.Match(num);
            //Console.WriteLine(m.Value);
            var w = q.ToList();
        }
    }
}
