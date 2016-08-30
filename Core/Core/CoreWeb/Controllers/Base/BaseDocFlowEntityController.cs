using BaseEntities;
using CoreAPI.Types;
using CoreWeb.Models.Catalog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Базовый контроллер, реализующий действия для управления плоским журналом объектов
    /// </summary>
    public abstract class BaseDocFlowEntityController : BaseFlatCatalogEntityController
    {
        /// <inheritdoc />
        public BaseDocFlowEntityController(ICoreDBContext db) : base(db) { }
        /// <summary>
        /// Получение интерфейса отображения колонок(полей)
        /// </summary>
        /// <returns>Интерфейс отображения полей</returns>
        protected override IColumnLoader GetColumnLoader()
        {
            return new DocFlowColumnLoader(CoreDB, this.CatalogParams) as IColumnLoader;
        }
        /// <summary>
        /// Получение модели журнала
        /// </summary>
        /// <returns>Модель журнала</returns>
        public new DocFlowModel GetModel()
        {
            ObjCatalog c = this.CatalogParams;
            if (c == null)
                throw new Exception("Ошибка загрузки настроек журнала.");
            DocFlowModel m = new DocFlowModel(this.GetColumnLoader());
            m.Controller = this.GetType().Name;
            m.Title = c.Name;
            m.ODataEntity = this.ODataEntity;
            m.States = this.CatalogParams.States.ToList();
            return m;
        }
        /// <inheritdoc />
        public override ActionResult Show()
        {
            return View("DocFlowCatalog", GetModel());
        }
        /// <inheritdoc />
        public override ActionResult ShowPartial()
        {
            return PartialView("DocFlowCatalogPartial", GetModel());
        }
    }
}
