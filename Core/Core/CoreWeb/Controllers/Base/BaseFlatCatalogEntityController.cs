using BaseEntities;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Mvc;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер, реализующий действия для управления плоским журналом объектов
    /// </summary>
    public abstract class BaseFlatCatalogEntityController : BaseEntityController
    {
        /// <inheritdoc />
        public BaseFlatCatalogEntityController(ICoreDBContext db) : base(db) { }
        /// <summary>
        /// Получение интерфейса отображения колонок(полей)
        /// </summary>
        /// <returns>Интерфейс отображения полей</returns>
        protected virtual IColumnLoader GetColumnLoader()
        {
            return new ColumnLoaderFromFields(CoreDB, this.CatalogParams.Fields) as IColumnLoader;
        }
        /// <summary>
        /// Возвращает наименование сущности, загружаемой в журнал
        /// </summary>
        protected virtual string ODataEntity { get { return this.GetType().Name.Replace("Controller", ""); } }
        /// <summary>
        /// Получение модели журнала
        /// </summary>
        /// <returns>Модель журнала</returns>
        public virtual CatalogModel GetModel()
        {
            ObjCatalog c = this.CatalogParams;
            if (c == null)
                throw new Exception("Ошибка загрузки настроек журнала.");
            CatalogModel m = new CatalogModel(this.GetColumnLoader());
            m.Controller = this.GetType().Name;
            m.Title = c.Name;
            m.ODataEntity = this.ODataEntity;
            return m;
        }
        /// <summary>
        /// Отображение плоского журнала, в качестве основной страницы
        /// </summary>
        /// <returns>Представление журнала</returns>
        public virtual ActionResult Show()
        {
            return View("FlatCatalog", GetModel());
        }
        /// <summary>
        /// Отображение плоского журнала, в качестве контента другой страницы
        /// </summary>
        /// <returns>Частичное представление журнала</returns>
        public virtual ActionResult ShowPartial()
        {
            return PartialView("FlatCatalogPartial", GetModel());
        }
        /// <summary>
        /// Получение интерфейса создания новой модели сущности
        /// </summary>
        /// <returns>Интерфейс создания новой модели сущности</returns>
        protected virtual INewObjectModel GetNewObjectModel()
        {
            return new BaseNewElementModel(_db, ParamGroupID) as INewObjectModel;
        }
        /// <summary>
        /// Отображение модального окна для добавления сущности
        /// </summary>
        /// <returns>Модальное окно добавления сущности</returns>
        [HttpPost]
        public virtual ActionResult PopupNew()
        {
            try
            {
                return PartialView("AddNewObject", GetNewObjectModel());
            }
            catch (Exception e)
            {
                return Content(ErrorManager.GetFullMessage(e));
            }
        }
        /// <summary>
        /// Реализация действия добавления сущности
        /// </summary>
        /// <param name="data">Новые данные</param>
        /// <returns>Результат добавления сущности</returns>
        protected abstract object IntNew(IEnumerable<ClientValue> data);
        /// <summary>
        /// Добавление новой сущности
        /// </summary>
        /// <param name="data">Данные сущности</param>
        /// <returns>Результат добавления</returns>
        [HttpPost]
        public ActionResult New(IEnumerable<ClientValue> data)
        {
            object o;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                o = IntNew(data);
                scope.Complete();
            }
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(o, new JavaScriptDateTimeConverter())));
        }
    }
}
