using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreDM;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.Catalog;
using CoreWeb.Models.EditModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CoreWeb.Models.Users;
using CoreWeb.Models.Roles;
using System.Transactions;
using CoreAPI;
using CoreWeb.Attributes.MVC;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления ролями
    /// </summary>
    [ExtAuthorize(AdminOnly = true)]
    public class RoleController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public RoleController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "7B74402E-BE39-4822-ABB1-A19CCC569DE1"; } }
        /// <inheritdoc />
        protected override IColumnLoader GetColumnLoader()
        {
            return new ColumnLoaderFromParams(CoreDB, 9654) as IColumnLoader;
        }
        /// <summary>
        /// Получение списка ролей
        /// </summary>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Список ролей</returns>
        protected override IQueryable GetObjectList(long groupID)
        {
            return CoreDB.SysUserRoles.AsNoTracking().Where(p => p.RoleID == groupID).Select(p => new { p.User.ID, p.User.Alias, p.User.FullName });
        }
        /// <summary>
        /// Получение списка ролей в JSON формате
        /// </summary>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Список ролей в JSON формате</returns>
        [HttpPost]
        public ActionResult GetMember(long groupID = 0)
        {
            return Content(JsonConvert.SerializeObject(GetObjectList(groupID == 0 ? RootGroupID.GetValueOrDefault() : groupID)));
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
                         f = 0,
                         spriteCssClass = "folder",
                         Discription = ""
                     }).Concat(from og in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == id)
                               join r in CoreDB.SysRoles.AsNoTracking() on og.ObjectID equals r.ID
                               select new { ID = r.ID, Name = r.Name, HasCld = false, f = 1, spriteCssClass = "role", Discription = r.Description }).OrderBy(p => new { p.f, p.Name });
            }
            return q;
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new RoleModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<ClientValue> data, long parentID)
        {
            SysRole element = new SysRole();
            InitObject(element, data);
            SysRoleOperations.New(CoreDB, parentID, element);
            return new { ID = element.ID, Name = element.Name, HasCld = false, f = 1, spriteCssClass = "role", Discription = element.Description };
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            SysRoleOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            SysRole element = CoreDB.SysRoles.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найдена роль (ID={0}).", id));
            InitObject(element, data);
            SysRoleOperations.Check(element);
            CoreDB.SaveChanges();
            return element;
        }
        /// <summary>
        /// Добавление пользователя к роли
        /// </summary>
        /// <param name="roleID">Идентификатор роли</param>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Добавленный пользователь в формате Json</returns>
        [HttpPost]
        public ActionResult AddMember(long roleID, long userID)
        {
            SysRoleOperations.AddUser(CoreDB, roleID, userID);
            var q = CoreDB.SysUsers.AsNoTracking().FirstOrDefault(p => p.ID == userID);
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(q, new JavaScriptDateTimeConverter())));
        }
        /// <summary>
        /// Получить список доменов службы Active Directory
        /// </summary>
        /// <returns>Список доменов службы Active Directory</returns>
        [HttpPost]
        public JsonResult GetDomens()
        {
            return Json(ADUtils.GetDomains(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Получить учетные данные пользователей из службы Active Directory
        /// </summary>
        /// <param name="domains">Список доменов (через запятую), в рамках которых производиться поиск</param>
        /// <param name="userName">Название учетной записи пользователя</param>
        /// <returns>Список учетных записей</returns>
        [HttpPost]
        public JsonResult GetADUsers(string domains, string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new List<ADUser>());
            }
            return Json(ADUtils.GetUsers(domains.Split(','), userName));
        }
        /// <summary>
        /// Добавление пользователя из Active Directory к роли
        /// </summary>
        /// <param name="roleID">Идентификатор роли</param>
        /// <param name="alias">Имя пользователя</param>
        /// <param name="domain">Домен пользователя</param>
        /// <returns>Добавленный пользователь в формате Json</returns>
        [HttpPost]
        public ActionResult AddMemberFromAD(long roleID, string alias, string domain)
        {
            ADUser acc = ADUtils.GetUser(domain, alias);
            if (acc == null)
            {
                throw new Exception(string.Format("Пользователь {0} не найден в домене {1}", alias, domain));
            }
            SysUser _sUser = CoreDB.SysUsers.FirstOrDefault(p => p.Alias == acc.Name.ToUpper());
            if (_sUser == null)
            {
                _sUser = new SysUser() { FullName = acc.FullName, Alias = acc.Name.ToUpper(), Name = acc.Name };
                SysUserOperations.New(CoreDB, _sUser);
            }
            else
            {
                if (string.IsNullOrEmpty(_sUser.FullName))
                {
                    _sUser.FullName = acc.FullName;
                }
            }
            SysRoleOperations.AddUser(CoreDB, roleID, _sUser.ID);
            return JavaScript(string.Format("{0}({1})", Request.Params["callback"], JsonConvert.SerializeObject(_sUser, new JavaScriptDateTimeConverter())));
        }
        /// <summary>
        /// Удаление пользователя из роли
        /// </summary>
        /// <param name="groupID">Идентификатор роли</param>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        [HttpPost]
        public ActionResult DelMember(long groupID, long id)
        {
            SysRoleOperations.DelUser(CoreDB, groupID, id);
            return Json("Успешно");
        }
        /// <summary>
        /// Возвращает представление включения пользователя в роль
        /// </summary>
        /// <returns>Представление</returns>
        public ActionResult PopupAddMember()
        {
            try
            {
                return PartialView();
            }
            catch (Exception e)
            {
                return Content(ErrorManager.GetFullMessage(e));
            }
        }
        /// <summary>
        /// Отображение представления количества прав доступа
        /// </summary>
        /// <returns>Представление</returns>
        public ActionResult PermissionCnt()
        {
            return PartialView();
        }

        /// <summary>
        /// Получение модели ролей объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Роли</returns>
        private IQueryable<ObjectRoleItemModel> GetRoleModel(long id)
        {
            var orm = CoreDB.SysObjectRoles.AsNoTracking().Where(p => p.ObjectID == id).Select(p => p.Role);
            return new ObjectRolesModel(CoreDB, RootGroupID.GetValueOrDefault(), "").Roles.Select(p => new ObjectRoleItemModel { ID = p.ID, Name = p.Name, Checked = orm.Any(s => s.ID == p.ID) }).AsQueryable();
        }
        /// <summary>
        /// Получение ролей объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Список ролей в формате Json</returns>
        [HttpPost]
        public ActionResult GetRolesObject(long id)
        {
            return Content(JsonConvert.SerializeObject(GetRoleModel(id)));
        }
        /// <summary>
        /// Отображение модального окна для редактирования ролей
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Представление добавления роли объекту</returns>
        [HttpPost]
        public virtual ActionResult PopupSetRoleObject(long id)
        {
            try
            {
                return PartialView("EditRoleObject", id);
            }
            catch (Exception e)
            {
                return Content(ErrorManager.GetFullMessage(e));
            }
        }
        /// <summary>
        /// Редактирование ролей объекта
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="roles">Роли объекта</param>
        /// <returns>Результат редактирования</returns>
        [HttpPost]
        public ActionResult SetObjectRoles(long id, long[] roles)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                if (roles == null) roles = new long[] { CoreConstant.EVERYONE_ROLE_ID };
                List<SysObjectRole> objRoles = CoreDB.SysObjectRoles.Where(p => p.ObjectID == id).ToList();
                foreach (SysObjectRole _s in objRoles.Where(p => !roles.Contains(p.RoleID)))
                {
                    CoreDB.SysObjectRoles.Remove(_s);
                }
                var _roles = roles.Where(p => !objRoles.Any(s => s.RoleID == p));
                foreach (var rolID in _roles)
                {
                    CoreDB.SysObjectRoles.Add(new SysObjectRole() { ObjectID = id, RoleID = rolID, OnRead = "T", OnUpdate = "F" });
                }
                CoreDB.SaveChanges();
                scope.Complete();
            }
            return Json("Success");
        }
    }
}
