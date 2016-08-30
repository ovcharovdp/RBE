using BaseEntities;
using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Пункт меню"
    /// </summary>
    public class SysMenuOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Пункт меню</param>
        public static void Check(SysMenu element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name))
                throw new Exception("Не задано название пункта меню.");
            if (string.IsNullOrEmpty(element.Type))
                throw new Exception("Не задан тип команды.");
        }
        /// <summary>
        /// Создает новый пункт меню
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор группы/родительского пункта меню</param>
        /// <param name="element">Пункт меню</param>
        /// <returns>Идентификатор созданного объекта</returns>
        public static long New(CoreEntities DB, long parentID, SysMenu element)
        {
            try
            {
                Check(element);
                if (element.ID == 0) element.ID = GetNextID(DB);
                DB.SysMenus.Add(element);
                if (DB.ObjGroups.AsNoTracking().FirstOrDefault(p => p.ID == parentID) == null)
                {
                    element.ParentID = parentID;
                }
                else
                {
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = element.ID });
                }
                SysRoleOperations.LinkObjectEveryone(DB, element.ID);
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при создании пункта меню.", e);
            }
            return element.ID;
        }
        /// <summary>
        /// Удаляет пункт меню
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор пункта меню</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (DB.SysMenus.AsNoTracking().Any(p => p.ParentID == id))
                        throw new Exception("Пункт мню содержит подпункты.");

                    SysMenu element = DB.SysMenus.Find(id);
                    if (element != null)
                    {
                        if (element.ParentID == null)
                            BaseObjectOperations.DelFromGroups(DB, id);

                        BaseObjectOperations.DelFromRoles(DB, id);
                        DB.SysMenus.Remove(element);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления пункта меню.", e);
                }
            }
        }
    }
}
