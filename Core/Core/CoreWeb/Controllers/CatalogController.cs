using BaseEntities;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreWeb.Attributes.MVC;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using CoreWeb.Models.EditModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Пространство имен для реализации контроллеров в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Контроллер управления объектами журнала
    /// </summary>
    [ExtAuthorize(AdminOnly = true)]
    public class CatalogController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public CatalogController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "1CF11323-E7EA-44BB-B181-23B6D82D05EA"; } }
        private long FieldParamGroupID
        {
            get
            {
                var q = CoreDB.ObjGroups.FirstOrDefault(p => p.Code.Equals("0020F27A-DB9B-408C-B41F-026FACFD1654"));
                if (q == null)
                    throw new Exception("Ошибка определения группы настроек полей");
                return q.ID;
            }
        }
        /// <inheritdoc />
        protected override IColumnLoader GetColumnLoader()
        {
            return new ColumnLoaderFromParams(CoreDB, FieldParamGroupID) as IColumnLoader;
        }
        /// <inheritdoc />
        protected override IQueryable GetGroupInt(long id, long baseID, short f)
        {
            IQueryable q;
            if (id == 0)
            {
                q = CoreDB.ObjGroups.AsNoTracking().Where(p => p.ID == baseID).Select(p => new { ID = p.ID, Name = p.Name, HasCld = true, expanded = true, f = 0 });
            }
            else
            {
                q = (from g in ObjGroupOperations.GetGroups(CoreDB, id)
                     select new
                     {
                         ID = g.ID,
                         Name = g.Name,
                         HasCld = CoreDB.ObjGroupObjects.Any(p => p.GroupID == g.ID),
                         f = "0",
                         spriteCssClass = "folder"
                     }).Concat(from og in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == id)
                               join r in CoreDB.ObjCataloges.AsNoTracking() on og.ObjectID equals r.ID
                               select new { ID = r.ID, Name = r.Name, HasCld = false, f = "Catalog", spriteCssClass = "catalog" }).OrderBy(p => new { p.f, p.Name });
            }
            return q;
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new CatalogFieldEditModel(_db, id, FieldParamGroupID) as IEditObjectModel;
        }
        /// <summary>
        /// Создание поля журнала
        /// </summary>
        /// <param name="parentID">Идентификатор журнала</param>
        /// <returns>Представление формы ввода</returns>
        public ActionResult PopupNewCatalog(long parentID)
        {
            return PartialView("AddNewObject", new BaseNewGroupedElementModel(_db, parentID, FieldParamGroupID) as INewObjectModel);
        }
        /// <summary>
        /// Редактирование журнала
        /// </summary>
        /// <param name="id">Идентификатор журнала</param>
        /// <returns>Представление формы редактирования</returns>
        [HttpPost]
        public ActionResult PopupEditCatalog(long id)
        {
            return PartialView("EditObjectPartial", new CatalogEditModel(_db, id, ParamGroupID));
        }
        /// <summary>
        /// Сохраняет изменения журнала
        /// </summary>
        /// <param name="data">Измененные поля</param>
        /// <param name="id">Идентификатор журнала</param>
        /// <returns>Созраненный объект</returns>
        [HttpPost]
        public ActionResult EditCatalog(IEnumerable<ClientValue> data, long id)
        {
            ObjCatalog o = CoreDB.ObjCataloges.Find(id);
            if (o == null)
                throw new Exception("Не найден журнал.");
            InitObject(o, data);
            ObjCatalogOperations.Check(o);
            CoreDB.SaveChanges();
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(o)));
        }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<ClientValue> data, long parentID)
        {
            ObjCatalog element = new ObjCatalog();
            InitObject(element, data);
            ObjCatalogOperations.New(CoreDB, parentID, element);
            return new { ID = element.ID, Name = element.Name, HasCld = false, f = "Catalog", spriteCssClass = "catalog" };
        }
        /// <summary>
        /// Создает новое поле журнала
        /// </summary>
        /// <param name="data">Атрибуты поля</param>
        /// <param name="parentID">Идентификатор журнала</param>
        /// <returns>Созданное поле</returns>
        [HttpPost]
        public ActionResult NewCatalog(IEnumerable<ClientValue> data, long parentID)
        {
            ObjCatalogField element = new ObjCatalogField();
            InitObject(element, data);
            ObjCatalogOperations.AddField(CoreDB, parentID, element);
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(element)));
        }
        /// <summary>
        /// Удаление журнала
        /// </summary>
        /// <param name="id">Идентификатор журнала</param>
        /// <returns>Результат удаления</returns>
        [HttpPost]
        public ActionResult DelCatalog(long id)
        {
            ObjCatalogOperations.Del(CoreDB, id);
            return Json("Success");
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            ObjCatalogOperations.DelField(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            ObjCatalogField element = CoreDB.ObjCatalogFields.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найден столбец журнала (ID={0}).", id));
            InitObject(element, data);
            CoreDB.SaveChanges();
            return element;
        }
        /// <inheritdoc />
        protected override IQueryable GetObjectList(long groupID)
        {
            var q = CoreDB.ObjCatalogFields.AsNoTracking().Where(p => p.Catalog.ID == groupID);
            return q;
        }
    }
}
