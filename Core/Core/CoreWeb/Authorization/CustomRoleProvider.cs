using System;
using System.Linq;
using System.Web.Security;
using CoreDM;
using CoreAPI.Types;
using System.Web.Mvc;

namespace CoreWeb.Authorization
{
    /// <summary>
    /// Класс, реализующий действия с ролями пользователей
    /// </summary>
    public class CustomRoleProvider : RoleProvider
    {
        private CoreEntities coreDB { get { return DependencyResolver.Current.GetService<ICoreDBContext>().CoreEntities; } }
        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        /// <param name="email">Имя пользователя</param>
        /// <returns>Список ролей</returns>
        public override string[] GetRolesForUser(string email)
        {
            var q = from u in coreDB.SysUsers.AsNoTracking().Where(rr => rr.Alias == email.ToUpper())
                    join ur in coreDB.SysUserRoles.AsNoTracking() on u.ID equals ur.UserID
                    select ur.Role.Name;
            return q.ToArray();
        }
        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Наличие роли у пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="roleName">Имя роли</param>
        /// <returns>Наличие роли у пользователя</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            var q = from u in coreDB.SysUsers.AsNoTracking().Where(rr => rr.Alias == username.ToUpper())
                    join ur in coreDB.SysUserRoles.AsNoTracking().Where(p => p.Role.Name.ToUpper() == roleName.ToUpper()) on u.ID equals ur.UserID
                    select ur.RoleID;
            return q.FirstOrDefault() > 0;
        }
        /// <summary>
        /// Добавление пользователям определенных ролей
        /// </summary>
        /// <param name="usernames">Список имен пользователей</param>
        /// <param name="roleNames">Список наименований ролей</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Имя приложения
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="roleName">Имя роли</param>
        /// <param name="throwOnPopulatedRole">Сбросить роли у всех пользователей</param>
        /// <returns>Результат удаления</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Поиск пользователей по поисковому запросу, принадлежащим определенная роль
        /// </summary>
        /// <param name="roleName">Наименование роли</param>
        /// <param name="usernameToMatch">Поисковый запрос имени</param>
        /// <returns>Список пользователей</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Получение всех ролей
        /// </summary>
        /// <returns>Список ролей</returns>
        public override string[] GetAllRoles()
        {
            return coreDB.SysRoles.AsNoTracking().Select(ss => ss.Name).ToArray();
        }
        /// <summary>
        /// Поиск всех пользователей, принадлежащим определенная роль
        /// </summary>
        /// <param name="roleName">Наименование роли</param>
        /// <returns>Список имен пользователей</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            var q = from r in coreDB.SysRoles.AsNoTracking().Where(rr => rr.Name.ToUpper() == roleName.ToUpper())
                    join ur in coreDB.SysUserRoles.AsNoTracking() on r.ID equals ur.RoleID
                    select ur.User.Name;
            return q.ToArray();
        }
        /// <summary>
        /// Удаление пользователей из определенных ролей
        /// </summary>
        /// <param name="usernames">Список пользователей</param>
        /// <param name="roleNames">Список ролей</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Наличие роли
        /// </summary>
        /// <param name="roleName">Наименование роли</param>
        /// <returns>Наличие роли</returns>
        public override bool RoleExists(string roleName)
        {
            var r = coreDB.SysRoles.AsNoTracking().Where(ss => ss.Name.ToUpper() == roleName.ToUpper()).FirstOrDefault();
            return r != null;
        }
    }
}