using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Роль доступа"
    /// </summary>
    public class SysRoleOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Роль</param>
        public static void Check(SysRole element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name))
                throw new Exception("Не задано название элемента.");
        }
        /// <summary>
        /// Создает новую роль
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <param name="element">Роль</param>
        /// <returns>Идентификатор роли</returns>
        public static long New(CoreEntities DB, long parentID, SysRole element)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (parentID <= 0)
                        throw new Exception("Не задана родительская группа.");
                    Check(element);

                    if (element.ID == 0)
                        element.ID = GetNextID(DB);
                    DB.SysRoles.Add(element);
                    //Создаем привязку к родителю
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = element.ID });
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании роли.", e);
                }
            }
            return element.ID;
        }
        /// <summary>
        /// Удаляет роль
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор роли</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (DB.SysUserRoles.Any(p => p.RoleID == id))
                        throw new Exception("Пользователям назначена эта роль.");

                    SysRole element = DB.SysRoles.Find(id);
                    if (element != null)
                    {
                        BaseObjectOperations.DelFromGroups(DB, id);
                        DB.SysRoles.Remove(element);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления роли.", e);
                }
            }
        }
        /// <summary>
        /// Связывает объект с ролью
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="objectID">Идентификатор объекта</param>
        /// <param name="roleID">Идентификатор роли</param>
        public static void LinkObject(CoreEntities DB, long objectID, long roleID)
        {
            SysObjectRole o = new SysObjectRole() { ObjectID = objectID, RoleID = roleID, OnRead = "T", OnUpdate = "F" };
            DB.SysObjectRoles.Add(o);
            DB.SaveChanges();
        }
        /// <summary>
        /// Связывает объект с ролью "Все"
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="objectID">Идентификатор объекта</param>
        public static void LinkObjectEveryone(CoreEntities DB, long objectID)
        {
            LinkObject(DB, objectID, CoreConstant.EVERYONE_ROLE_ID);
        }
        /// <summary>
        /// Удаляет привязку пользователя к роли
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="roleID">Идентификатор роли</param>
        /// <param name="userID">Идентификатор пользователя</param>
        public static void DelUser(CoreEntities DB, long roleID, long userID)
        {
            SysUserRole r = new SysUserRole() { RoleID = roleID, UserID = userID };
            DB.SysUserRoles.Attach(r);
            DB.SysUserRoles.Remove(r);
            DB.SaveChanges();
        }
        /// <summary>
        /// Добавляет пользователя к роли
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="roleID">Идентификатор роли</param>
        /// <param name="userID">Идентификатор пользователя</param>
        public static void AddUser(CoreEntities DB, long roleID, long userID)
        {
            var t = DB.SysUserRoles.AsNoTracking().Where(p => p.RoleID == roleID && p.UserID == userID).FirstOrDefault();
            if (t == null)
            {
                DB.SysUserRoles.Add(new SysUserRole() { RoleID = roleID, UserID = userID, StartDate = DateTime.Today });
                DB.SaveChanges();
            }
        }
    }
}
