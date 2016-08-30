using CoreDM;
using System;
using System.Linq;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс для реализации базовых операций с объектом БД.
    /// </summary>
    public class BaseObjectOperations : BaseDBOperations
    {
        /// <summary>
        /// Удаление привязки объекта к группам
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор объекта</param>
        public static void DelFromGroups(CoreEntities DB, long id)
        {
            try
            {
                var q = DB.ObjGroupObjects.Where(p => p.ObjectID == id);
                DB.ObjGroupObjects.RemoveRange(q);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления привязки объекта к группе.", e);
            }
        }
        /// <summary>
        /// Удаление привязки объекта к ролям
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор объекта</param>
        public static void DelFromRoles(CoreEntities DB, long id)
        {
            try
            {
                var q = DB.SysObjectRoles.Where(p => p.ObjectID == id);
                DB.SysObjectRoles.RemoveRange(q);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка удаления привязки объекта к ролям доступа.", e);
            }
        }
    }
}
