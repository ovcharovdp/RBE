using BaseEntities;
using CoreAPI.Const;
using CoreAPI.Operations;
using CoreDM;
using FuelAPI.Operations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;

namespace FuelAPI.Logistica
{
    public class LogisticaChuv
    {
        CoreEntities _db;
        SysDictionary _orderPlanState;
        SysDictionary _azsType;
        long _tankFarmGroupID;
        long _UTTGroupID;
        long _modelGroupID;
        Dictionary<string, OrgDepartment> _tankFarmList;
        Dictionary<string, OrgDepartment> _uttList;
        Dictionary<string, OrgDepartment> _filialList;
        Dictionary<string, SysDictionary> _modelList;
        List<TRNAuto> _autoList;
        Dictionary<string, SysDictionary> _gsmList;
        List<FlStation> _stationList;
        SqlConnection c;
        SqlConnection c1;
        SqlCommand itemCmd;
        System.IO.StreamWriter _file;
        Dictionary<string, SysDictionary> _states;

        public LogisticaChuv(StreamWriter file)
        {
            _file = file;
        }
        /// <summary>
        /// Возвращает Нефтебазу
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор нефтебазы</param>
        /// <param name="name">Название нефтебязы из Логистики</param>
        /// <returns>Нефтебаза</returns>
        private OrgDepartment getTankFarm(CoreEntities db, string id, string name)
        {
            if (!_tankFarmList.ContainsKey(id))
            {
                OrgDepartment org = new OrgDepartment() { Code = id, Name = name, TypeID = 213 };
                OrgOperations.New(db, _tankFarmGroupID, org);
                _tankFarmList[id] = org;
            }
            return _tankFarmList[id];
        }
        /// <summary>
        /// Возвращает перевозчика
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор перевозчика</param>
        /// <param name="name">Наименование перевозчика</param>
        /// <returns>Перевозчик</returns>
        private OrgDepartment getUTT(CoreEntities db, string id, string name)
        {
            if (!_uttList.ContainsKey(id))
            {
                OrgDepartment org = new OrgDepartment() { Code = id, Name = name, TypeID = 205 };
                OrgOperations.New(db, _UTTGroupID, org);
                _uttList[id] = org;
            }
            return _uttList[id];
        }
        private SysDictionary getModel(CoreEntities db, string id, string name)
        {
            if (!_modelList.ContainsKey(id))
            {
                SysDictionary d = new SysDictionary() { Code = id, Name = name };
                long i = SysDictionaryOperations.New(db, _modelGroupID, d);
                _modelList[id] = d;// db.SysDictionaries.Find(i);
            }
            return _modelList[id];
        }
        private TRNAuto getAuto(CoreEntities db, IDataReader r)
        {
            string num = r["RegNum"].ToString();
            TRNAuto a = _autoList.FirstOrDefault(p => p.RegNum.StartsWith(num));
            if (a == null)
            {
                OrgDepartment utt = getUTT(db, r["idUTT"].ToString(), r["UTT"].ToString());
                SysDictionary model = getModel(db, r["idMarka"].ToString(), r["Marka"].ToString());
                a = new TRNAuto() { Model = model, NextCertDate = DateTime.Today, Organization = utt, RegNum = num };
                AutoOperations.New(db, a);
                _autoList.Add(a);
            }
            return a;
        }
        private FlStation getStation(IDataReader data)
        {

            //Regex rgx = new Regex("[0-9]{1,3}", RegexOptions.IgnoreCase);
            //Match m = rgx.Match(data["azs"].ToString());
            //if (String.IsNullOrEmpty(m.Value))
            //{
            //    throw new Exception("Номер АЗС не определен: " + data["azs"].ToString());
            //}
            //short number = Convert.ToInt16(m.Value);
            //string org = data["org_code"].ToString();
            //_stationList.SingleOrDefault(p => p.Number == number && p.Organization.Code.Equals(org));
            FlStation s = _stationList.SingleOrDefault(p => p.InfoOilCode == data["id_azsasutp"].ToString());
            if (s == null)
            {
                throw new Exception("Код АЗС не задан (" + data["id_azsasutp"] + ")");
                //s = new FlStation()
                //{
                //    Name = m.Value.PadLeft(3, '0'),
                //    Code = 0,
                //    Address = data["Address"].ToString(),
                //    Number = number,
                //    Organization = _filialList[org],
                //    Type = _azsType
                //};
                //StationOperations.New(_db, s);
                //_stationList.Add(s);
            }
            //if (String.IsNullOrEmpty(s.Address))
            //{
            //    s.Address = data["Address"].ToString();
            //}
            return s;
        }
        private void createOrderItem(FlOrder order)
        {
            Dictionary<byte, bool> s = new Dictionary<byte, bool>();
            itemCmd.Parameters["@mkID"].Value = order.LogID;
            using (IDataReader rr = itemCmd.ExecuteReader())
            {
                while (rr.Read())
                {
                    if (s.ContainsKey(Convert.ToByte(rr["SectionID"]))) break;
                    s[Convert.ToByte(rr["SectionID"])] = true;
                    try
                    {
                        if (string.IsNullOrEmpty(rr["idgsmasutp"].ToString()))
                        {
                            throw new Exception("ГСМ;" + rr["idgsmasutp"].ToString());
                        }

                        FlStation station = getStation(rr);
                        string productCode = rr["idgsmasutp"].ToString();
                        byte sectionNum = Convert.ToByte(rr["SectionID"]);
                        SysDictionary stateCanceled = _states["2"];
                        FlOrderItem item = order.Items.FirstOrDefault(p => p.SectionNum == sectionNum && p.State.ID != stateCanceled.ID);
                        if (item == null)
                        {
                            FlOrderItem newItem = new FlOrderItem()
                            {
                                SectionNum = sectionNum,
                                TankNum = 0,
                                Volume = Convert.ToInt16(rr["obem"]),
                                Station = station,
                                Product = _gsmList[productCode],
                                State = _states["1"],
                                Customer = station.Organization,
                                ReceiveDate = DateTime.Now
                            };
                            OrderOperations.AddItem(_db, newItem);
                            order.Items.Add(newItem);
                            order.Volume += newItem.Volume;
                        }
                        else
                        {
                            if (item.Station.ID != station.ID || !item.Product.Code.Equals(productCode))
                            {
                                FlOrderItem newItem = new FlOrderItem()
                                {
                                    SectionNum = sectionNum,
                                    TankNum = 0,
                                    Volume = Convert.ToInt16(rr["obem"]),
                                    Station = station,
                                    Product = _gsmList[productCode],
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
                                    WaybillDate = item.WaybillDate
                                };
                                OrderOperations.AddItem(_db, newItem);
                                order.Items.Add(newItem);
                                if (item.State.Code.Equals("3"))
                                {
                                    // отправить ЭТТН
                                }
                                item.State = _states["2"];
                                item.ReceiveDate = DateTime.Now;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _file.WriteLine(e.Message);
                    }
                }
                rr.Close();
            }
        }
        private void createOrder(IDataReader r, byte orderID)
        {
            int logID = (int)r["idMKDet"];
            DateTime planTime = Convert.ToDateTime(r["Nachalo"]);
            TRNAuto auto = getAuto(_db, r);
            DateTime orderDate = (DateTime)r["Data"];
            FlOrder order = _db.FlOrders.Include("Items").Where(p => p.Auto.ID == auto.ID && p.DocDate == orderDate && p.Order == orderID).FirstOrDefault();
            OrgDepartment tankFarm = getTankFarm(_db, r["idNPU"].ToString(), r["NPU"].ToString());
            if (order == null)
            {
                order = new FlOrder() { FillDatePlan = planTime, Auto = auto, DocDate = orderDate, LogID = logID, Order = orderID, TankFarm = tankFarm, State = _states["1"] };
                OrderOperations.New(_db, order);
                createOrderItem(order);
            }
            else
            {
                // if (order.LogID == logID) return;
                order.TankFarm = tankFarm;
                order.FillDatePlan = planTime;
                order.LogID = logID;
                createOrderItem(order);
            }
        }
        private void Init()
        {
            IConstLoader l = new GroupIDLoader(_db);
            _tankFarmGroupID = l.Load("8E6BB8D5-52E3-48E3-AD7E-61071CCAF7FC");
            var q = from og in _db.ObjGroupObjects.Where(p => p.GroupID == _tankFarmGroupID)
                    join o in _db.OrgDepartments on og.ObjectID equals o.ID
                    select o;
            _tankFarmList = q.ToDictionary(p => p.Code, p => p);

            _UTTGroupID = l.Load("A160B3B8-52D3-43F4-8DB0-ACF01A2F6344");
            q = _db.OrgDepartments.Where(p => p.Type.ID == 205 && p.Code != null);
            _uttList = q.ToDictionary(p => p.Code, p => p);

            _modelGroupID = l.Load("F2D2896C-B93E-4FF8-A7EF-8BBFB70E1868");
            var m = from og in _db.ObjGroupObjects.Where(p => p.GroupID == _modelGroupID)
                    join d in _db.SysDictionaries on og.ObjectID equals d.ID
                    select d;
            _modelList = m.ToDictionary(p => p.Code, p => p);
            _autoList = _db.TRNAutos.ToList();

            var gsmGroupID = l.Load("73A6CA9F-4630-43C7-A37B-CF18535809E3");
            m = from og in _db.ObjGroupObjects.Where(p => p.GroupID == gsmGroupID)
                join d in _db.SysDictionaries on og.ObjectID equals d.ID
                select d;
            _gsmList = m.ToDictionary(p => p.Code, p => p);

            _orderPlanState = _db.SysDictionaries.Find(200);
            _azsType = _db.SysDictionaries.Find(150);
            _stationList = _db.FlStations.Include("Organization").ToList();
            _filialList = _db.OrgDepartments.Where(p => p.ParentID == 102).ToDictionary(p => p.Code, p => p);

            _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
        }
        public int UploadWaybills()
        {
            _db = new CoreEntities();
            Init();
            var con = ConfigurationManager.ConnectionStrings["ChuvDS"].ToString();
            c1 = new SqlConnection(con);

            c = new SqlConnection(con);
            using (c)
            {
                string orderSQL =
                    @"SELECT mkd.Nachalo,g.id_object as idUTT,replace(g.Naimen,'Гараж ','') as UTT,upper(a.Nomer) as RegNum," +
                    " case n.id_object when 249 then 318 when 283 then 318 when 320 then 6 when 377 then 485 else n.id_object end idNPU,n.Naimen as NPU," +
                    " mk.id_mk as idMK,id_mkavto as idMKDet,mk.Data," +
                    " case a.Marka when 'Scania' then 52 when 'КамАЗ' then 57 when 'ГАЗ' then 54 when 'МАЗ' then 59 else 0 end as idMarka,a.Marka" +
                    " FROM [tMK_Avto] mkd,[tMK] mk,[tBenzovoz] a,[tPredpriyatiya] g,[tPredpriyatiya] n" +
                    " where mk.Data>=GetDate()-1" +
                    " and id_dejstvie='Пломбировка секций'" +
                    " and mk.id_mk=mkd.id_mk" +
                    " and mk.id_avto=a.id_avto" +
                    " and g.id_object=a.id_object" +
                    " and n.id_object=mkd.id_object1" +
                    //     " and upper(a.Nomer) = 'Т396ХА'"+
                    " order by mk.id_mk,mkd.Nachalo";
                SqlCommand oCmd = new SqlCommand(orderSQL, c);
                string itemSql =
                    "SELECT azs.id_azsasutp,s.nomer_sekcii as SectionID,s.obem * 1000 obem," +
                    " case mkd.id_gsm when 1 then 80 when 2 then 92 when 3 then 95 when 28 then 98 when 4 then 50 else 0 end as idgsmasutp" +
                    " FROM [tMK_Avto] mko,[tMK_Avto] mkd,[tAZSReCode] azs,[tSekcii] s" +
                    " where mko.id_mkavto=@mkID" +
                    " and mkd.id_mk = mko.id_mk" +
                    " and mkd.id_dejstvie = 'Слив'" +
                    " and mkd.Nachalo >mko.Nachalo" +
                    " and azs.id_azs = mkd.id_object1" +
                    " and s.n_sekcii = mkd.n_sekcii" +
                    " order by  mkd.Nachalo";
                itemCmd = new SqlCommand(itemSql, c1);
                itemCmd.Parameters.AddWithValue("@mkID", 0);
                c.Open();
                c1.Open();
                using (SqlDataReader r = oCmd.ExecuteReader())
                {
                    byte order = 1;
                    int prevMKid = 0;
                    while (r.Read())
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                if (prevMKid == (int)r["idMK"]) { order++; }
                                else
                                {
                                    prevMKid = (int)r["idMK"];
                                    order = 1;
                                }

                                createOrder(r, order);
                                _db.SaveChanges();
                                scope.Complete();
                            }
                            catch (Exception e)
                            {
                                _file.WriteLine(e.Message);
                            }
                        }
                    }
                    c.Close();
                }
            }
            return 0;
        }
    }
}
