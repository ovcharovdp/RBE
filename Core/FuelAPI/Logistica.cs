﻿using BaseEntities;
using CoreAPI.Const;
using CoreAPI.Operations;
using CoreDM;
using FuelAPI.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace FuelAPI
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
        System.IO.StreamWriter file;
        Dictionary<string, SysDictionary> _states;


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
            Regex rgx = new Regex("[0-9]{1,3}", RegexOptions.IgnoreCase);
            Match m = rgx.Match(data["azs"].ToString());
            if (String.IsNullOrEmpty(m.Value))
            {
                throw new Exception("Номер АЗС не определен: " + data["azs"].ToString());
            }
            short number = Convert.ToInt16(m.Value);
            string org = data["org_code"].ToString();
            FlStation s = _stationList.SingleOrDefault(p => p.Number == number && p.Organization.Code.Equals(org));
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
                        FlOrderItem item = order.Items.FirstOrDefault(p => p.SectionNum == sectionNum && p.State.ID != stateCanceled.ID);
                        if (item == null)
                        {
                            FlOrderItem newItem = new FlOrderItem()
                            {
                                // OrderID = order.ID,
                                SectionNum = sectionNum,
                                TankNum = Convert.ToByte(rr["nomer"]),
                                Volume = Convert.ToInt16(Convert.ToDecimal(rr["obem"]) * 1000),
                                Station = station,
                                Product = _gsmList[productCode],
                                State = _states["1"],
                                Customer = station.Organization,
                                ReceiveDate = DateTime.Now
                            };
                            OrderOperations.AddItem(_db, newItem);
                            order.Items.Add(newItem);
                        }
                        else
                        {
                            if (item.Station.ID != station.ID || !item.Product.Code.Equals(productCode))
                            {
                                FlOrderItem newItem = new FlOrderItem()
                                {
                                    SectionNum = sectionNum,
                                    TankNum = Convert.ToByte(rr["nomer"]),
                                    Volume = Convert.ToInt16(Convert.ToDecimal(rr["obem"]) * 1000),
                                    Station = station,
                                    Product = _gsmList[productCode],
                                    State = _states[item.State.Code],
                                    Customer = station.Organization,
                                    Density = item.Density,
                                    QPassportDate = item.QPassportDate,
                                    QPassportNum = item.QPassportNum,
                                    ReceiveDate = DateTime.Now,
                                    Temperature = item.Temperature,
                                    VolumeFact = item.VolumeFact,
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
                        file.WriteLine(e.Message);
                    }
                }
                rr.Close();
            }
        }
        private void createOrder(IDataReader r)
        {
            int logID = (int)r["idMKDet"];
            TRNAuto auto = getAuto(_db, r);
            DateTime orderDate = (DateTime)r["Data"];
            byte o = Convert.ToByte(r["order_id"]);
            FlOrder order = _db.FlOrders.Include("Items").Where(p => p.Auto.ID == auto.ID && p.DocDate == orderDate && p.Order == o).FirstOrDefault();
            OrgDepartment tankFarm = getTankFarm(_db, r["idNPU"].ToString(), r["NPU"].ToString());
            if (order == null)
            {
                order = new FlOrder() { Auto = auto, DocDate = orderDate, LogID = logID, Order = o, TankFarm = tankFarm };
                OrderOperations.New(_db, order);
                createOrderItem(order);
            }
            else
            {
               // if (order.LogID == logID) return;
                order.TankFarm = tankFarm;
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
            q = from og in _db.ObjGroupObjects.Where(p => p.GroupID == _UTTGroupID)
                join o in _db.OrgDepartments.Where(p => p.Code != null) on og.ObjectID equals o.ID
                select o;
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

            _states = OrderOperations.GetStates(_db).ToDictionary(p => p.Code, p => p);
        }
        public int UploadWaybills()
        {
            file = new System.IO.StreamWriter("c:\\Denis\\errors.txt");
            _db = new CoreEntities();
            Init();
            //SqlConnection c = new SqlConnection("Data Source=logistika;Initial Catalog=NewLogistics;Persist Security Info=True;User ID=OPER;Password=admin_new");
            var con = "Data Source=logistika;Initial Catalog=NewLogistics;Persist Security Info=True;User ID=OPER;Password=admin_new";
            // ConfigurationManager.ConnectionStrings["Yourconnection"].ToString();
            c1 = new SqlConnection(con);

            c = new SqlConnection(con);
            using (c)
            {
                //string oString = "Select * from Employees where FirstName=@fName";
                string orderSQL =
                    @"SELECT replace(replace(replace(replace(a.Nomer,'RUS',''),' ','-'),'RUC',''),'US','') as RegNum,case when n.idNPU=319 then 318 else n.idNPU end as idNPU,n.NPU,mk.idMK, mkd.idMKDet, mk.Data, " +
                    "  case when m.idMarka=64 then 52 " +
                    "  when m.idMarka = 66 then 57 when m.idMarka = 68 then 67 else m.idMarka end as idMarka, " +
                    "  DENSE_RANK() over (partition by mk.idMK, a.Nomer order by mkd.Nachalo) as order_id," +
                    "  m.Marka, u.idUTT,replace(u.UTT,'Гараж ','') as UTT, replace(replace(replace(replace(a.PPricepNumber, '-', ''), ' 16RUS', '-16'), ' 116RUS', '-116'), ' ', '') as ExtRegNum" +
                    "  FROM [tMK] mk,[sAvto] a,[tMKDet] mkd,[sNPU] n,[sMarka] m,[sUTT] u " +
                    " where mk.[Data] >= GetDate()-1" +
                    "   and a.idAvto= mk.idAvto" +
                    "   and mkd.idMK= mk.idMK" +
                    "   and mkd.idDejst = 5" +
                    "   and n.[idNPU]= idPredpr2" +
                    "   and a.idMarka = m.idMarka" +
                    "   and u.idUTT = a.idUTT" +
                      // "   and a.Nomer like 'У344%'" +
                    " order by idMK, Nachalo";
                SqlCommand oCmd = new SqlCommand(orderSQL, c);
                string itemSql =
                    "SELECT DENSE_RANK() over (order by mk.idSekcii) as SectionID, r.nomer, s.obem,g.idgsmasutp,g.idGSM,a.azs,a.idazs, p.Address, " +
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
                                file.WriteLine(e.Message);
                            }
                        }
                    }
                    c.Close();
                }
            }
            file.Close();
            // _db.SaveChanges();
            return 0;
        }
    }
}