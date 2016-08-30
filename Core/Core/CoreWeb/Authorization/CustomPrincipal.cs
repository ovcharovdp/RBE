using CoreAPI.Authorization;
using CoreAPI.Operations;
using CoreAPI;
using CoreDM;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;
using CoreWeb.Models.Users;

namespace CoreWeb.Authorization
{
    /// <summary>
    /// Пространство имен для реализации средств авторизации пользователя в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс, реализующий действия со сведениями о пользователе
    /// </summary>
    public class CustomPrincipal : ICustomPrincipal
    {
        private long _id;
        private long[] _roleIDs;
        /// <inheritdoc/>
        public long[] RoleIDs { get { return _roleIDs; } }
        /// <summary>
        /// Базовые функциональные возможности объекта идентификации
        /// </summary>
        public System.Security.Principal.IIdentity Identity { get; private set; }
        /// <summary>
        /// Возвращает признак назначения пользователю роли доступа
        /// </summary>
        /// <param name="role">Название роли</param>
        /// <returns>Признак назначения пользователю роли доступа</returns>
        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated &&
                !string.IsNullOrWhiteSpace(role) && Roles.IsUserInRole(Identity.Name, role);
        }
        /// <inheritdoc/>
        public string FirstName { get; set; }
        /// <inheritdoc/>
        public string LastName { get; set; }
        /// <inheritdoc/>
        public bool isAdmin { get; set; }
        /// <inheritdoc/>
        public bool IsBlocked { get; set; }
        private string _fullName;
        /// <inheritdoc/>
        public string FullName { get { return _fullName; } }
        /// <inheritdoc/>
        public DateTime LastLogon { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="user">Пользователь</param>
        /// <param name="principal">Учетная запись пользователя</param>
        public CustomPrincipal(CoreEntities db, IPrincipal user, UserPrincipal principal)
        {
            this.Identity = new System.Security.Principal.GenericIdentity(user.Identity.Name);
            var u = db.SysUsers.Include("State").FirstOrDefault(p => p.Alias.Equals(user.Identity.Name.ToUpper()));
            if (u == null)
            {
                u = new SysUser()
                {
                    Alias = user.Identity.Name.ToUpper(),
                    Name = user.Identity.Name,
                    LastLogon = DateTime.Today
                };
                SysUserOperations.New(db, u);
            }
            else if (u.LastLogon != DateTime.Today)
            {
                u.LastLogon = DateTime.Today;
                db.SaveChanges();
            }
            if (string.IsNullOrEmpty(u.FullName) && principal != null)
            {
                u.FullName = string.IsNullOrEmpty(principal.DisplayName) ? string.Join(" ", new string[] { principal.GivenName, principal.Surname }) : principal.DisplayName;
                db.SaveChanges();
            }
            if (string.IsNullOrEmpty(u.FullName))
            {
                ADUser uAD = ADUtils.GetUser(u.Name.Split('\\')[0], u.Name.Split('\\')[1]);
                if (uAD != null)
                {
                    u.FullName = uAD.FullName;
                    db.SaveChanges();
                }
            }
            _roleIDs = (new long[] { CoreConstant.EVERYONE_ROLE_ID }).Concat(db.SysUserRoles.AsNoTracking().Where(p => p.UserID == u.ID).Select(p => p.RoleID)).ToArray();
            long[] inheritRole;
            do
            {
                inheritRole = db.SysInheritRoles.AsNoTracking().Where(p => _roleIDs.Contains(p.HeirRoleID) && !_roleIDs.Contains(p.RoleID)).Select(p => p.RoleID).ToArray();
                if (inheritRole.Length > 0) _roleIDs = _roleIDs.Concat(inheritRole).ToArray();
            } while (inheritRole.Length > 0);
            _id = u.ID;
            _fullName = u.FullName;
            this.IsBlocked = u.State.Code.Equals("LOC");
            this.isAdmin = !this.IsBlocked && u.IsAdmin;
        }
        /// <inheritdoc/>
        public long ID { get { return _id; } }
    }
}