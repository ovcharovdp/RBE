using CoreAPI.Operations;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Areas.Org.Models;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Areas.Org.Controllers
{
    /// <summary>
    /// Пространство имен для описания контроллеров работы с организациями.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Контролер отображение представления организаций
    /// </summary>
    public class DepartmentController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public DepartmentController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "6A9A68FD-9363-4769-8F40-6641C5BEC67D"; } }
        //protected override long RootGroupID { get { return 9798; } }
        /// <inheritdoc />
        protected override IQueryable GetGroupInt(long id, long baseID, short f)
        {
            IQueryable q;

            if (id == 0)
                q = CoreDB.ObjGroups.AsNoTracking().Where(p => p.ID == baseID).Select(p => new { ID = p.ID, Name = p.Name, HasCld = true, expanded = true, f = 0 });
            else
            {
                if (!CoreDB.OrgDepartments.Any(p => p.ID == id))
                {
                    q = (from g in ObjGroupOperations.GetGroups(CoreDB, id)
                         select new
                         {
                             ID = g.ID,
                             Name = g.Name,
                             HasCld = CoreDB.ObjGroupObjects.Any(p => p.GroupID == g.ID),
                             f = 0,
                             spriteCssClass = "folder"
                         }).Concat(from og in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == id)
                                   join r in CoreDB.OrgDepartments.AsNoTracking() on og.ObjectID equals r.ID
                                   select new { ID = r.ID, Name = r.StructName, HasCld = r.Childs.Any(), f = 1, spriteCssClass = "org" }).OrderBy(p => new { p.f, p.Name });
                }
                else
                {
                    q = from o in CoreDB.OrgDepartments.AsNoTracking().Where(p => p.ParentID == id)
                        orderby o.Name
                        select new { ID = o.ID, Name = o.StructName, HasCld = o.Childs.Any(), f = 1, spriteCssClass = "org" };
                }
            }
            return q;
        }
        /// <summary>
        /// Получение списка полей организации
        /// </summary>
        /// <returns>Список полей организации</returns>
        protected override IColumnLoader GetColumnLoader() { return null; }
        /// <summary>
        /// Создание новой организации
        /// </summary>
        /// <param name="data">Данные новой организации</param>
        /// <param name="parentID">Идентификатор родительской организации</param>
        /// <returns>Результат создания новой организации</returns>
        protected override object IntNew(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long parentID)
        {
            OrgDepartment element = new OrgDepartment();
            InitObject(element, data);
            OrgOperations.New(CoreDB, parentID, element);
            return new { ID = element.ID, Name = element.StructName, HasCld = false, f = 1, spriteCssClass = "org" };
        }
        /// <summary>
        /// Получение интерфейса создания организации
        /// </summary>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <returns>Интерфейс новой модели объекта</returns>
        protected override INewObjectModel GetNewObjectModel(long parentID)
        {
            return new NewDepartmentModel(_db, parentID, ParamGroupID) as INewObjectModel;
        }
        //   protected override long ParamGroupID { get { return 9789; } }
        /// <summary>
        /// Удаление организации
        /// </summary>
        /// <param name="id">Идентификатор организации</param>
        protected override void IntDel(long id)
        {
            OrgOperations.Del(CoreDB, id);
        }
        /// <summary>
        /// Изменение организациии
        /// </summary>
        /// <param name="data">Новые данные организации</param>
        /// <param name="id">Идентификатор организации</param>
        /// <returns>Измененная организация</returns>
        protected override object IntEdit(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long id)
        {
            OrgDepartment element = CoreDB.OrgDepartments.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найдена редактируемая организация (ID={0}).", id));
            InitObject(element, data);
            OrgOperations.Check(element);
            OrgOperations.Refresh(CoreDB, element);
            CoreDB.SaveChanges();
            return new { ID = element.ID, Name = element.StructName, f = 1, spriteCssClass = "org" };
        }
        /// <summary>
        /// Получение интерфейса редактирования объекта
        /// </summary>
        /// <param name="id">Идентификатор организации</param>
        /// <returns>Интерфейс редактирования организации</returns>
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new EditDepartmentModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
    }
}
