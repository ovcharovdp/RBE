using CoreWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreAPI.Types;

namespace CoreWeb.Areas.Transport.Controllers
{
    public class AutoController : BaseFlatCatalogEntityController
    {
        public AutoController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID
        {
            get { return "A88D31FD-27E0-4AA3-9D73-7B1EC05D6957"; }
        }

        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            throw new NotImplementedException();
        }

        protected override string ODataEntity
        {
            get
            {
                return base.ODataEntity;
            }
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