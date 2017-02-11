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
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

namespace FuelAPI.Logistica
{
    public class Logistica
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
        Dictionary<string, TRNAuto> _autoList;
        Dictionary<string, SysDictionary> _gsmList;
        List<FlStation> _stationList;
        SqlConnection c;
        SqlConnection c1;
        SqlCommand itemCmd;
        System.IO.StreamWriter _file;
        Dictionary<string, SysDictionary> _states;

        public Logistica(System.IO.StreamWriter file)
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
            if (!_autoList.ContainsKey(num))
            {
                OrgDepartment utt = getUTT(db, r["idUTT"].ToString(), r["UTT"].ToString());
                SysDictionary model = getModel(db, r["idMarka"].ToString(), r["Marka"].ToString());
                TRNAuto a = new TRNAuto() { Model = model, NextCertDate = DateTime.Today, Organization = utt, RegNum = num, RegNumExt = r["ExtRegNum"].ToString() };
                AutoOperations.New(db, a);
                _autoList[num] = a;
            }
            return _autoList[num];
        }
        private FlStation getStation(IDataReader data)
        {
            FlStation s;
            Regex rgx = new Regex("(АЗС){1,}(.+)[0-9]{1,3}", RegexOptions.IgnoreCase);
            Match m = rgx.Match(data["azs"].ToString());
            if (String.IsNullOrEmpty(m.Value))
            {
                string id = data["idazs"].ToString();
                s = _stationList.FirstOrDefault(p => p.InfoOilCode == id);
                if (s == null)
                {
                    string name = data["azs"].ToString();
                    name = name.Length > 20 ? name.Substring(0, 19) : name;
                    s = new FlStation()
                    {
                        Name = name,
                        Code = 0,
                        Address = data["Address"].ToString(),
                        Number = 0,
                        Organization = _filialList["777"],
                        Type = _azsType,
                        InfoOilCode = id
                    };
                    StationOperations.New(_db, s);
                    _stationList.Add(s);
                }
            }
            else
            {
                rgx = new Regex("[0-9]{1,3}", RegexOptions.IgnoreCase);
                m = rgx.Match(m.Value);
                short number = Convert.ToInt16(m.Value);
                string org = data["org_code"].ToString();
                s = _stationList.SingleOrDefault(p => p.Number == number && p.Organization.Code.Equals(org));

                if (s == null)
                {
                    s = new FlStation()
                    {
                        Name = m.Value.PadLeft(3, '0'),
                        Code = 0,
                        Address = data["Address"].ToString(),
                        Number = number,
                        Organization = _filialList[org],
                        Type = _azsType
                    };
                    StationOperations.New(_db, s);
                    _stationList.Add(s);
                }
            }
            if (String.IsNullOrEmpty(s.Address))
            {
                s.Address = data["Address"].ToString();
            }
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
                            throw new Exception("ГСМ;" + rr["idGSM"].ToString());
                        }

                        FlStation station = getStation(rr);
                        string productCode = rr["idgsmasutp"].ToString();
                        byte sectionNum = Convert.ToByte(rr["SectionID"]);
                        SysDictionary stateCanceled = _states["2"];
                        FlOrderItem item = order.Items.FirstOrDefault(p => p.SectionNum == sectionNum && ((p.State.ID != stateCanceled.ID && !p.IsChanged) || p.IsChanged));
                        if (item == null)
                        {
                            FlOrderItem newItem = new FlOrderItem()
                            {
                                SectionNum = sectionNum,
                                TankNum = Convert.ToByte(rr["nomer"]),
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
                            if (item.IsChanged)
                                continue;

                            if (item.Station.ID != station.ID || !item.Product.Code.Equals(productCode))
                            {
                                FlOrderItem newItem = new FlOrderItem()
                                {
                                    SectionNum = sectionNum,
                                    TankNum = Convert.ToByte(rr["nomer"]),
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
        private void createOrder(IDataReader r)
        {
            int logID = (int)r["idMKDet"];
            DateTime planTime = Convert.ToDateTime(r["Nachalo"]);
            TRNAuto auto = getAuto(_db, r);
            DateTime orderDate = (DateTime)r["Data"];
            byte o = Convert.ToByte(r["order_id"]);
            FlOrder order = _db.FlOrders.Include("Items").Where(p => p.Auto.ID == auto.ID && p.DocDate == orderDate && p.Order == o).FirstOrDefault();
            OrgDepartment tankFarm = getTankFarm(_db, r["idNPU"].ToString(), r["NPU"].ToString());
            if (order == null)
            {
                order = new FlOrder() { FillDatePlan = planTime, Auto = auto, DocDate = orderDate, LogID = logID, Order = o, TankFarm = tankFarm, State = _states["1"] };
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
            _autoList = _db.TRNAutos.ToDictionary(p => p.RegNum, p => p);

            var gsmGroupID = l.Load("73A6CA9F-4630-43C7-A37B-CF18535809E3");
            m = from og in _db.ObjGroupObjects.Where(p => p.GroupID == gsmGroupID)
                join d in _db.SysDictionaries on og.ObjectID equals d.ID
                select d;
            _gsmList = m.ToDictionary(p => p.Code, p => p);

            _orderPlanState = _db.SysDictionaries.Find(200);
            _azsType = _db.SysDictionaries.Find(150);
            _stationList = _db.FlStations.Include("Organization").ToList();
            _filialList = _db.OrgDepartments.Where(p => p.ParentID == 102).ToDictionary(p => p.Code, p => p);
            _filialList.Add("777", _db.OrgDepartments.FirstOrDefault(p => p.ID == 82));

            _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
        }
        public int UploadWaybills()
        {
            _db = new CoreEntities();
            Init();
            var con = ConfigurationManager.ConnectionStrings["LogisticDS"].ToString();
            c1 = new SqlConnection(con);

            c = new SqlConnection(con);
            using (c)
            {
                string orderSQL =
                    @"SELECT mkd.Nachalo,replace(replace(replace(replace(a.Nomer,'RUS',''),' ','-'),'RUC',''),'US','') as RegNum,case n.idNPU when 319 then 318 when 463 then 485 else n.idNPU end as idNPU,n.NPU,mk.idMK, mkd.idMKDet, mk.Data, " +
                    " case m.idMarka when 64 then 52 when 66 then 57 when 68 then 67 else m.idMarka end as idMarka," +
                    "  DENSE_RANK() over (partition by mk.idMK, a.Nomer order by mkd.Nachalo) as order_id," +
                    "  m.Marka, u.idUTT,replace(u.UTT,'Гараж ','') as UTT, replace(replace(replace(replace(a.PPricepNumber, '-', ''), ' 16RUS', '-16'), ' 116RUS', '-116'), ' ', '') as ExtRegNum" +
                    "  FROM [tMK] mk,[sAvto] a,[tMKDet] mkd,[sNPU] n,[sMarka] m,[sUTT] u " +
                    " where mk.[Data] >= GetDate()-1" +
                    "   and a.idAvto= mk.idAvto" +
                    "   and mkd.idMK= mk.idMK" +
                    "   and mkd.idDejst = 6" +
                    "   and n.[idNPU]= idPredpr2" +
                    "   and a.idMarka = m.idMarka" +
                    "   and u.idUTT = a.idUTT" +
                    " and mk.status=2" +
                    // "   and a.Nomer like '%362%'" +
                    " order by idMK, Nachalo";
                SqlCommand oCmd = new SqlCommand(orderSQL, c);
                string itemSql =
                    "SELECT mk.idMKDET, DENSE_RANK() over (order by mk.idSekcii) as SectionID, r.nomer, s.obem*1000 obem,case g.idGSM when 20 then 109 when 30 then 102 else g.idgsmasutp end as idgsmasutp,g.idGSM," +
                    " a.azs,case a.idazs when 448 then 267 when 454 then 267 when 458 then 267 when 459 then 460 else a.idazs end idazs,p.Address, " +
                    " case p.idFilial when 1 then 116 when 2 then 216 when 3 then 416 when 4 then 73 when 5 then 63 when 6 then 18 when 7 then 12 when 8 then 316 when 9 then 21 else 0 end as org_code" +
                    "  FROM [tMKDet] mk,[tMKDet] mko,[sAZS] a,[sReservuar] r,[sSekcii] s,[sGSM] g,[sPredpr] p " +
                    " where mko.idMKDet=@mkID " +
                    "   and mk.idDejst=3 " +
                    "   and mk.idMK = mko.idMK" +
                    "   and mk.Nachalo > mko.Nachalo" +
                    "   and a.idAZS = mk.idPredpr2" +
                    "   and r.idReservuar = mk.idReservuar" +
                    "   and s.idSekcii = mk.idSekcii" +
                    "   and g.idgsm= mk.idgsm" +
                    "   and p.idPredpr = mk.idPredpr2" +
                    " order by mk.Nachalo";
                itemCmd = new SqlCommand(itemSql, c1);
                itemCmd.Parameters.AddWithValue("@mkID", 0);
                c.Open();
                c1.Open();
                using (SqlDataReader r = oCmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                createOrder(r);
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
