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
using CoreWeb.Areas.Transport.Models;

namespace CoreWeb.Areas.Transport.Controllers
{
    public class DriverController : BaseFlatCatalogEntityController
    {
        public DriverController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "E0EA4364-1526-4A6A-BC57-A1D2B8D5E833"; } }
        protected override string ODataEntity { get { return "TRNDrivers"; } }
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            throw new NotImplementedException();
        }
        protected override IColumnLoader GetColumnLoader()
        {
            return new DriverCatalogColumnLoader(CoreDB, this.CatalogParams.Fields) as IColumnLoader;
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