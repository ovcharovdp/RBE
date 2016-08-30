using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreDM;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using CoreWeb.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления отчетами
    /// </summary>
    public class ReportController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public ReportController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "71016D7E-BAF8-4608-9CC5-8F1A3DAFE407"; } }
        //protected override IColumnLoader GetColumnLoader() { return new ColumnLoaderFromFields(CoreDB, 72664) as IColumnLoader; }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<ClientValue> data, long parentID)
        {
            SysReport element = new SysReport();
            InitObject(element, data);
            SysReportOperations.New(CoreDB, parentID, element);
            return element;
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new ReportEditModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            SysReportOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            SysReport element = CoreDB.SysReports.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найден редактируемый отчет (ID={0}).", id));
            InitObject(element, data);
            SysReportOperations.Check(element);
            CoreDB.SaveChanges();
            return element;
        }
        /// <inheritdoc />
        protected override IQueryable GetObjectList(long groupID)
        {
            return from g in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == groupID)
                   join d in CoreDB.SysReports.AsNoTracking() on g.ObjectID equals d.ID
                   select new { d.ID, d.Name, d.Description };
        }
        /// <summary>
        /// Отображение представления отчета
        /// </summary>
        /// <param name="id">Идентификатор отчета</param>
        /// <returns>Представление отчета</returns>
        public ActionResult Run(long id)
        {
            return View(new RunReportModel(CoreDB, id));
        }
    }
}
