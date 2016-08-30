using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using CoreDM;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.Params;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.EditModels;
using CoreWeb.Models.BaseEditModels;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreWeb.Attributes.MVC;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления параметрами
    /// </summary>
    //[ExtAuthorize(AdminOnly = true)]
    public class ParamController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public ParamController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "8701B02D-5329-44CD-BD87-121B3C8CCA31"; } }
        /// <summary>
        /// Отображение представления списка параметров
        /// </summary>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Представление списка параметров</returns>
        public ActionResult LoadParams(long parentID, long groupID = -2)
        {
            return View((new ParamListModel(_db, parentID, groupID)).Params);
        }
        /// <summary>
        /// Возвращает контент, содержащий значения параметров
        /// </summary>
        /// <param name="parentID">Идентификатор объекта-владельца значений</param>
        /// <param name="groupID">Группа параметров</param>
        /// <returns>Контент</returns>
        public ActionResult LoadParamsPartial(long parentID, long groupID = -2)
        {
            return PartialView((new ParamListModel(_db, parentID, groupID)).Params);
        }
        /// <summary>
        /// Возвращает контент, содержащий значения определенных пользователем параметров
        /// </summary>
        /// <param name="param">Параметры</param>
        /// <returns>Контент</returns>
        public ActionResult LoadCustomParamsPartial(IEnumerable<ParamSettings> param)
        {
            return PartialView("LoadParamsPartial", param);
        }
        /// <summary>
        /// Отображение представления для редактирования списка параметров, как основной страницы
        /// </summary>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Представление для редактирования списка параметров</returns>
        public ActionResult EditParams(long parentID, long groupID = -2)
        {
            return View((new ParamListModel(_db, parentID, groupID)).Params);
        }
        /// <summary>
        /// Отображение частиного представления для редактирования списка параметров, как контента другой страницы
        /// </summary>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Частичное представление для редактирования списка параметров</returns>
        public ActionResult EditParamsPartial(long parentID, long groupID = -2)
        {
            return PartialView((new ParamListModel(_db, parentID, groupID)).Params);
        }
        /// <summary>
        /// Пользовательское частичное представление для редактирования параметров, в виде контента другой страницы 
        /// </summary>
        /// <param name="param">Список параметров</param>
        /// <returns>Пользовательское частичное представление для редактирования параметров</returns>
        public ActionResult EditCustomParamsPartial(IEnumerable<ParamSettings> param)
        {
            return PartialView("EditParamsPartial", param);
        }

        //[HttpPost]
        //public ActionResult GetControl(long parentID, long id, int order)
        //{
        //    return PartialView("EditControl", new EditControlModel() { Order = order, Settings = new ParamSettings(id) { ParentID = parentID } });
        //}
        /// <summary>
        /// Отображение частиного представления мультиредактирования строк
        /// </summary>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="id">Идентификатор параметра</param>
        /// <param name="order">Порядок</param>
        /// <returns>Представление для мультиредактирования значений параметра</returns>
        [HttpPost]
        public ActionResult GetMultirowControl(long parentID, long id, int order)
        {
            return PartialView("MultirowEditControl", new EditControlModel() { Order = order, Settings = new ParamSettings(CoreDB.ObjParams.Find(id)) { ParentID = parentID } });
        }
        /// <inheritdoc />
        protected override IQueryable GetObjectList(long groupID)
        {
            return from g in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == groupID)
                   join d in CoreDB.ObjParams.AsNoTracking()
                   on g.ObjectID equals d.ID
                   select d;
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new EditParamModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<ClientValue> data, long parentID)
        {
            ObjParam param = new ObjParam();
            InitObject(param, data);
            ObjParamOperations.New(CoreDB, parentID, param);
            return param;
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            ObjParamOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            ObjParam param = CoreDB.ObjParams.Find(id);
            if (param == null)
                throw new Exception(string.Format("Не найден редактируемый параметр (ID={0}).", id));
            InitObject(param, data);
            ObjParamOperations.Check(param);
            CoreDB.SaveChanges();
            return param;
        }
    }
}
