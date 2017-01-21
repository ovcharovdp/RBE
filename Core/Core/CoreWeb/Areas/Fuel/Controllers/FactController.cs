using CoreWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreAPI.Types;
using CoreWeb.Areas.Fuel.Models;
using CoreDM;
using FuelAPI.Fact;

namespace CoreWeb.Areas.Fuel.Controllers
{
    public class FactController : BaseFlatCatalogEntityController
    {
        public FactController(ICoreDBContext db) : base(db) { }
        protected override string CatalogGID { get { return "82A10F5C-CD17-47FA-B66B-318E7370F876"; } }
        protected override string ODataEntity { get { return "FlFacts"; } }
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new FactEditModel(_db, id, ParamGroupID) as IEditObjectModel;
        }

        protected override void IntDel(long id)
        {
            throw new NotImplementedException();
        }

        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            FlFact fact = CoreDB.FlFacts.Find(id);
            if (fact == null)
                throw new Exception(string.Format("Не найден редактируемый объект (ID={0}).", id));
            InitObject(fact, data);
            FactHandler handler = new FactHandler(CoreDB);
            handler.Handle(fact);
            CoreDB.SaveChanges();
            return fact;
        }

        protected override object IntNew(IEnumerable<ClientValue> data)
        {
            throw new NotImplementedException();
        }
    }
}