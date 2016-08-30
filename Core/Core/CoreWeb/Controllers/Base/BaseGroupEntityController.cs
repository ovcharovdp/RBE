using BaseEntities;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер, реализующий действия для управления журналом объектов, разбитым по группам
    /// </summary>
    public abstract class BaseGroupEntityController : BaseEntityController
    {
        /// <inheritdoc />
        public BaseGroupEntityController(ICoreDBContext db) : base(db) { }
        /// <summary>
        /// Реализация действия добавления сущности
        /// </summary>
        /// <param name="data">Новые данные</param>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <returns>Результат добавления сущности</returns>
        protected abstract object IntNew(IEnumerable<ClientValue> data, long parentID);
        /// <summary>
        /// Получение интерфейса отображения колонок(полей)
        /// </summary>
        /// <returns>Интерфейс отображения полей</returns>
        protected virtual IColumnLoader GetColumnLoader()
        {
            if (CatalogParams.Fields.Count > 0)
                return new ColumnLoaderFromFields(CoreDB, CatalogParams.Fields);
            else
                return new ColumnLoaderFromParams(CoreDB, ParamGroupID) as IColumnLoader;
        }
        /// <summary>
        /// Получение модели групп
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <returns>Модель</returns>
        public GroupedListModel GetModel(long id)
        {
            ObjCatalog c = this.CatalogParams;
            GroupedListModel m = new GroupedListModel(this.GetColumnLoader(),
                id == 0 ? c.RootID.GetValueOrDefault() : id);
            m.Controller = this.GetType().Name;
            m.Title = c.Name;
            return m;
        }
        /// <summary>
        /// Отображение групп сущностей, в качестве основной страницы
        /// </summary>
        /// <param name="groupID">Идентификатор группы, с которой начинается прогрузка элементов</param>
        /// <returns>Представление групп сущностей</returns>
        public virtual ActionResult Show(long groupID = 0)
        {
            return View("GroupedCatalog", GetModel(groupID));
        }
        /// <summary>
        /// Отображение групп сущностей, в качестве контента другой страницы
        /// </summary>
        /// <param name="groupID">Идентификатор группы, с которой начинается прогрузка элементов</param>
        /// <returns>Частичное представление групп сущностей</returns>
        public virtual ActionResult ShowPartial(long groupID = 0)
        {
            return PartialView("GroupedCatalogPartial", GetModel(groupID));
        }
        /// <summary>
        /// Добавление новой сущности в группу
        /// </summary>
        /// <param name="data">Данные сущности</param>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <returns>Результат добавления</returns>
        [HttpPost]
        public ActionResult New(IEnumerable<ClientValue> data, long parentID)
        {
            object o;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                o = IntNew(data, parentID);
                scope.Complete();
            }
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(o, new JavaScriptDateTimeConverter())));
        }
        /// <summary>
        /// Получение списка объектов группы
        /// </summary>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Список объектов группы</returns>
        protected virtual IQueryable GetObjectList(long groupID) { return null; }
        /// <summary>
        /// Получение элементов группы
        /// </summary>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Список объектов в формате Json</returns>
        [HttpPost]
        public ActionResult GetData(long groupID = 0)
        {
            return Content(JsonConvert.SerializeObject(GetObjectList(groupID == 0 ? this.RootGroupID.GetValueOrDefault() : groupID)));
        }
        /// <summary>
        /// Получение вложенных групп
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <param name="baseID">Базовая группа</param>
        /// <param name="f">Дополнительная метка раскрываемого объекта
        /// <note type="note">С помощью метки можно определять тип раскрываемого элемента. Это позволит оптимизировать логику формирования дочерних элементов.</note>
        /// </param>
        /// <returns>Список групп</returns>
        protected virtual IQueryable GetGroupInt(long id, long baseID, short f)
        {
            IQueryable q;
            if (id == 0)
            {
                q = from g in CoreDB.ObjGroups.AsNoTracking().Where(p => p.ID == baseID)
                    select new { ID = g.ID, Name = g.Name, HasCld = true, expanded = true };
            }
            else
            {
                q = from g in ObjGroupOperations.GetGroups(CoreDB, id)
                    orderby g.Name
                    select new
                    {
                        ID = g.ID,
                        Name = g.Name,
                        HasCld = (from og1 in CoreDB.ObjGroupObjects.Where(p => p.GroupID == g.ID)
                                  join g1 in CoreDB.ObjGroups on og1.ObjectID equals g1.ID
                                  select 1).Any(),
                        spriteCssClass = "folder"
                    };
            }
            return q;
        }
        /// <summary>
        /// Получение дочерних элементов группы, отображаемых в дереве
        /// </summary>
        /// <param name="baseID">Идентификатор группы (раскрытой)</param>
        /// <param name="ID">Идентификатор объекта</param>
        /// <param name="f">Дополнительная метка раскрываемого объекта
        /// <note type="note">С помощью метки можно определять тип раскрываемого элемента. Это позволит оптимизировать логику формирования дочерних элементов.</note>
        /// </param>
        /// <returns>Список дочерних групп в формате JSON</returns>
        [HttpPost]
        public JsonResult GetGroups(long baseID, long ID = 0, short f = 0)
        {
            return Json(GetGroupInt(ID, baseID, f));
        }
        /// <summary>
        /// Получение интерфейса создания новой модели сущности
        /// </summary>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <returns>Модель создания сущности</returns>
        protected virtual INewObjectModel GetNewObjectModel(long parentID)
        {
            return new BaseNewGroupedElementModel(_db, parentID, ParamGroupID) as INewObjectModel;
        }
        /// <summary>
        /// Отображение модального окна для добавления сущности
        /// </summary>
        /// <param name="parentID">Идентификатор группы, в которую добавляется элемент</param>
        /// <returns>Модальное окно добавления сущности</returns>
        [HttpPost]
        public virtual ActionResult PopupNew(long parentID)
        {
            try
            {
                return PartialView("AddNewObject", GetNewObjectModel(parentID));
            }
            catch (Exception e)
            {
                return Content(ErrorManager.GetFullMessage(e));
            }
        }
    }
}
