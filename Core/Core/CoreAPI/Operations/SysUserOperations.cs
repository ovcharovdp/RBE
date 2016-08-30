using CoreDM;
using System;
using System.Data.Entity;
using System.Linq;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Пользователь"
    /// </summary>
    public class SysUserOperations : BaseDBOperations
    {
        static long _crtStateID = 0;
        static long getStartStateID(CoreEntities DB)
        {
            if (_crtStateID == 0)
            {
                var q = DB.ObjCataloges.FirstOrDefault(p => p.GID.Equals(GUID)).States.FirstOrDefault(p => p.Code.Equals("CRT"));
                if (q == null)
                {
                    throw new Exception("Не удалось определить начальное состояние.");
                }
                _crtStateID = q.ID;
            }
            return _crtStateID;
        }
        /// <inheritdoc />
        private static string GUID { get { return "6042C665-9912-4877-B8EE-6A301E80956C"; } }
        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="user">Пользователь</param>
        public static void New(CoreEntities DB, SysUser user)
        {
            try
            {
                if (user.ID == 0)
                    user.ID = GetNextID(DB);
                user.State = DB.ObjCatalogStates.Find(getStartStateID(DB));
                DB.SysUsers.Add(user);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании пользователя.", e);
            }
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="user">Пользователь</param>
        public static void Update(CoreEntities DB, SysUser user)
        {
            try
            {
                DB.SysUsers.Attach(user);
                DB.Entry(user).State = EntityState.Modified;
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при изменении пользователя.", e);
            }
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор роли</param>
        public static void Del(CoreEntities DB, long id)
        {
            try
            {
                if (DB.SysUserRoles.Any(p => p.UserID == id))
                    throw new Exception("Пользователям имеет роль. Удаление невозможно.");

                SysUser element = DB.SysUsers.Find(id);
                if (element != null)
                {
                    DB.SysUsers.Remove(element);
                    DB.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления роли.", e);
            }
        }
    }
}