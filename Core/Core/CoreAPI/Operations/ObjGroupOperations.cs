using System;
using System.Linq;
using System.Transactions;
using CoreDM;
using CoreAPI.Extensions;
using BaseEntities;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Группа"
    /// </summary>
    public class ObjGroupOperations : BaseDBOperations
    {
        /// <summary>
        /// Возвращает группы в соответствии с правами доступа
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="groupID">Идентификатор родительской группы</param>
        /// <returns>Список доступных групп</returns>
        public static IQueryable<ObjGroup> GetGroups(CoreEntities DB, long groupID = -1)
        {
            var q = from og in DB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == groupID)
                    join g in DB.GetAllowObjects<ObjGroup, CoreDM.SysObjectRole>() on og.ObjectID equals g.ID
                    select g;
            return q;
        }
        /// <summary>
        /// Проверка корректности заполнения данных
        /// </summary>
        /// <param name="group">Группа</param>
        public static void Check(ObjGroup group)
        {
            if (string.IsNullOrEmpty(group.Name))
                throw new Exception("Не задано название группы.");
        }
        /// <summary>
        /// Создает новую группу
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <param name="group">Группа</param>
        /// <returns>Идентификатор группы</returns>
        public static long New(CoreEntities DB, long parentID, ObjGroup group)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (parentID <= 0)
                        throw new Exception("Не задана родительская группа");

                    Check(group);

                    if (group.ID == 0)
                        group.ID = GetNextID(DB);
                    DB.ObjGroups.Add(group);
                    //Создаем привязку к родителю
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = group.ID });
                    SysRoleOperations.LinkObjectEveryone(DB, group.ID);
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании группы.", e);
                }
            }
            return group.ID;
        }
        /// <summary>
        /// Удаление группы
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор группы</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ObjGroup group = DB.ObjGroups.Find(id);
                    if (group != null)
                    {
                        //получаем привязки - дочерние группы
                        if (DB.ObjGroupObjects.Any(p => p.GroupID == id))
                            throw new Exception("Группа содержит дочерние элементы.");

                        BaseObjectOperations.DelFromGroups(DB, id);
                        BaseObjectOperations.DelFromRoles(DB, id);

                        DB.ObjGroups.Remove(group);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления группы.", e);
                }
            }
        }
        /// <summary>
        /// Добавляет объект в группу
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="groupID">Идентификатор группы</param>
        /// <param name="objectID">Идентификатор объекта</param>
        public static void Add(CoreEntities DB, long groupID, long objectID)
        {
            try
            {
                DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = groupID, ObjectID = objectID });
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка добавления объекта в группу.", e);
            }
        }
    }
}