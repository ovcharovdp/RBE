using CoreWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreAPI.Types;
using CoreWeb.Models.Catalog;
using CoreWeb.Areas.Fuel.Models;
using CoreDM;
using FuelAPI.Operations;

namespace CoreWeb.Areas.Fuel.Controllers
{
    public class StationController : BaseFlatCatalogEntityController
    {
        public StationController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "3B938323-8CD9-400C-86C1-90BB69B47BB7"; } }
        protected override string ODataEntity { get { return "FlStations"; } }
        protected override IColumnLoader GetColumnLoader()
        {
            return new StationCatalogColumnLoader(CoreDB, this.CatalogParams.Fields) as IColumnLoader;
        }
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new StationEditModel(_db, id, ParamGroupID) as IEditObjectModel;
        }

        protected override void IntDel(long id)
        {
            StationOperations.Del(_db, id);
        }

        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            FlStation station = CoreDB.FlStations.Find(id);
            if (station == null)
                throw new Exception(string.Format("Не найдена редактируемая АЗС (ID={0}).", id));
            InitObject(station, data);
            CoreDB.SaveChanges();
            return station;
        }

        protected override object IntNew(IEnumerable<ClientValue> data)
        {
            throw new NotImplementedException();
        }
    }
}