using BaseEntities;
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
using System.Threading.Tasks;

namespace FuelAPI
{
    public static class Logistica
    {
        static SysDictionary _orderPlanState;
        static long _tankFarmGroupID;
        static long _UTTGroupID;
        static long _modelGroupID;
        static Dictionary<string, OrgDepartment> _tankFarmList;
        static Dictionary<string, OrgDepartment> _uttList;
        static Dictionary<string, SysDictionary> _modelList;
        static Dictionary<string, TRNAuto> _autoList;
        /// <summary>
        /// Возвращает Нефтебазу
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор нефтебазы</param>
        /// <param name="name">Название нефтебязы из Логистики</param>
        /// <returns>Нефтебаза</returns>
        private static OrgDepartment getTankFarm(CoreEntities db, string id, string name)
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
        private static OrgDepartment getUTT(CoreEntities db, string id, string name)
        {
            if (!_uttList.ContainsKey(id))
            {
                OrgDepartment org = new OrgDepartment() { Code = id, Name = name, TypeID = 205 };
                OrgOperations.New(db, _UTTGroupID, org);
                _uttList[id] = org;
            }
            return _uttList[id];
        }
        private static SysDictionary getModel(CoreEntities db, string id, string name)
        {
            if (!_modelList.ContainsKey(id))
            {
                SysDictionary d = new SysDictionary() { Code = id, Name = name };
                long i = SysDictionaryOperations.New(db, _modelGroupID, d);
                _modelList[id] = d;// db.SysDictionaries.Find(i);
            }
            return _modelList[id];
        }
        private static TRNAuto getAuto(CoreEntities db, IDataReader r)
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

        private static void createOrder(CoreEntities db, IDataReader r)
        {
            int logID = (int)r["idMKDet"];
            if (!db.FlOrders.Any(p => p.LogID == logID))
            {
                Console.WriteLine(logID);
                OrgDepartment tankFarm = getTankFarm(db, r["idNPU"].ToString(), r["NPU"].ToString());
                TRNAuto auto = getAuto(db, r);
                DateTime orderDate = (DateTime)r["Data"];
                int o = db.FlOrders.Count(p => p.Auto.ID == auto.ID && p.DocDate == orderDate && p.TankFarm.ID == tankFarm.ID) + 1;
                FlOrder order = new FlOrder() { Auto = auto, DocDate = orderDate, LogID = logID, Order = (byte)o, TankFarm = tankFarm, State = _orderPlanState };
                OrderOperations.New(db, order);
            }
        }
        private static void Init(CoreEntities db)
        {
            IConstLoader l = new GroupIDLoader(db);
            _tankFarmGroupID = l.Load("8E6BB8D5-52E3-48E3-AD7E-61071CCAF7FC");
            var q = from og in db.ObjGroupObjects.Where(p => p.GroupID == _tankFarmGroupID)
                    join o in db.OrgDepartments on og.ObjectID equals o.ID
                    select o;
            _tankFarmList = q.ToDictionary(p => p.Code, p => p);

            _UTTGroupID = l.Load("A160B3B8-52D3-43F4-8DB0-ACF01A2F6344");
            var u = from og in db.ObjGroupObjects.Where(p => p.GroupID == _UTTGroupID)
                    join o in db.OrgDepartments.Where(p => p.Code != null) on og.ObjectID equals o.ID
                    select o;
            _uttList = u.ToDictionary(p => p.Code, p => p);

            _modelGroupID = l.Load("F2D2896C-B93E-4FF8-A7EF-8BBFB70E1868");
            var m = from og in db.ObjGroupObjects.Where(p => p.GroupID == _modelGroupID)
                    join d in db.SysDictionaries on og.ObjectID equals d.ID
                    select d;
            _modelList = m.ToDictionary(p => p.Code, p => p);
            _autoList = db.TRNAutos.ToDictionary(p => p.RegNum, p => p);

            _orderPlanState = db.SysDictionaries.Find(200);
        }
        public static int UploadWaybills()
        {
            CoreEntities e = new CoreEntities();
            Init(e);
            //SqlConnection c = new SqlConnection("Data Source=logistika;Initial Catalog=NewLogistics;Persist Security Info=True;User ID=OPER;Password=admin_new");
            var con = "Data Source=logistika;Initial Catalog=NewLogistics;Persist Security Info=True;User ID=OPER;Password=admin_new";
            // ConfigurationManager.ConnectionStrings["Yourconnection"].ToString();

            using (SqlConnection c = new SqlConnection(con))
            {
                //string oString = "Select * from Employees where FirstName=@fName";
                string oString =
                    "SELECT replace(replace(replace(a.Nomer,'RUS',''),' ','-'),'RUC','') as RegNum,n.idNPU,n.NPU,mk.idMK, mkd.idMKDet, mk.Data, " +
"case when m.idMarka = 64 then 52 " +
"when m.idMarka = 66 then 57 " +
"when m.idMarka = 68 then 67 " +
"else m.idMarka end as idMarka, " +
" m.Marka, u.idUTT,replace(u.UTT,'Гараж ','') as UTT, replace(replace(replace(replace(a.PPricepNumber, '-', ''), ' 16RUS', '-16'), ' 116RUS', '-116'), ' ', '') as ExtRegNum" +
"  FROM [tMK] mk,[sAvto] a,[tMKDet] mkd,[sNPU] n,[sMarka] m,[sUTT] u " +
"where mk.[Data] >= GetDate()" +
"  and a.idAvto= mk.idAvto" +
"  and mkd.idMK= mk.idMK" +
"  and mkd.idDejst = 5" +
"  and n.[idNPU]= idPredpr2" +
"  and a.idMarka = m.idMarka" +
"  and u.idUTT = a.idUTT" +
" order by idMK, Nachalo";
                SqlCommand oCmd = new SqlCommand(oString, c);
                //oCmd.Parameters.AddWithValue("@Fname", fName);
                c.Open();
                using (IDataReader r = oCmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        createOrder(e, r);
                    }
                    c.Close();
                }
            }
            e.SaveChanges();
            return 0;
        }
    }
}
