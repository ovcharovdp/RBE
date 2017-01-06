using CoreWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreAPI.Types;

namespace CoreWeb.Areas.Fuel.Controllers
{
    public class TankController : BaseFlatCatalogEntityController
    {
        public TankController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "3D536DD6-C1DE-48FA-B828-C1E9064A9BCA"; } }
        protected override string ODataEntity { get { return "FlStationTanks"; } }
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            throw new NotImplementedException();
        }

        protected override void IntDel(long id)
        {
            throw new NotImplementedException();
        }

        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            throw new NotImplementedException();
        }

        protected override object IntNew(IEnumerable<ClientValue> data)
        {
            throw new NotImplementedException();
        }
    }
}