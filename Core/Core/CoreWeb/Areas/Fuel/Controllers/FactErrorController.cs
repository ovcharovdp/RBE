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
    public class FactErrorController : BaseFlatCatalogEntityController
    {
        public FactErrorController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "82A10F5C-CD17-47FA-B66B-318E7370F876"; } }
        protected override string ODataEntity { get { return "FlFactErrors"; } }
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