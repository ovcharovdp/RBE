using CoreAPI.Operations;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Attributes.MVC;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using CoreWeb.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления меню
    /// </summary>
    [ExtAuthorize(AdminOnly = true)]
    public class MenuController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public MenuController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "00E95911-D8B8-4585-A30E-6D57A064437A"; } }
        /// <inheritdoc />
        protected override IQueryable GetGroupInt(long id, long baseID, short f)
        {
            IQueryable q;

            if (id == 0)
                q = CoreDB.ObjGroups.AsNoTracking().Where(p => p.ID == baseID).Select(p => new { ID = p.ID, Name = p.Name, HasCld = true, expanded = true, f = 0 });
            else
            {
                if (CoreDB.SysMenus.AsNoTracking().FirstOrDefault(p => p.ID == id) == null)
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
                                   join r in CoreDB.SysMenus.AsNoTracking() on og.ObjectID equals r.ID
                                   select new { ID = r.ID, Name = r.Name, HasCld = r.Childs.Any(), f = 1, spriteCssClass = "menu-item" }).OrderBy(p => new { p.f, p.Name });
                }
                else
                {
                    q = from o in CoreDB.SysMenus.AsNoTracking().Where(p => p.ParentID == id)
                        orderby o.Name
                        select new { ID = o.ID, Name = o.Name, HasCld = o.Childs.Any(), f = 1, spriteCssClass = "menu-item" };
                }
            }
            return q;
        }
        /// <inheritdoc />
        protected override IColumnLoader GetColumnLoader() { return null; }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long parentID)
        {
            SysMenu element = new SysMenu();
            InitObject(element, data);
            SysMenuOperations.New(CoreDB, parentID, element);
            return new { ID = element.ID, Name = element.Name, HasCld = false, f = 1, spriteCssClass = "menu-item" };
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            SysMenuOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long id)
        {
            SysMenu element = CoreDB.SysMenus.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найден редактируемый пункт меню (ID={0}).", id));
            InitObject(element, data);
            SysMenuOperations.Check(element);
            CoreDB.SaveChanges();
            return new { ID = element.ID, Name = element.Name, f = 1, spriteCssClass = "menu-item" };
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new MenuEditModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
    }
}
