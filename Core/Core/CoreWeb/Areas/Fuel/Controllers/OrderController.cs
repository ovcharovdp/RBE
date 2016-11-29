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

namespace CoreWeb.Areas.Fuel.Controllers
{
    public class OrderController : BaseFlatCatalogEntityController
    {
        public OrderController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "AE6C7990-A560-4C68-85D1-C8D95BDA9077"; } }
        protected override string ODataEntity { get { return "FlOrderItems"; } }
        protected override IColumnLoader GetColumnLoader()
        {
            return new OrderCatalogColumnLoader(CoreDB, this.CatalogParams.Fields) as IColumnLoader;
        }
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