using BaseEntities;
using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Словарь"
    /// </summary>
    public class SysDictionaryOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Элемент словаря</param>
        public static void Check(SysDictionary element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name))
                throw new Exception("Не задано название элемента.");
            if (string.IsNullOrEmpty(element.Code))
                throw new Exception("Не задан код элемента.");
        }
        /// <summary>
        /// Создает новый элемент словаря
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <param name="element">Элемент словаря</param>
        /// <returns>Идентификатор элемента словаря</returns>
        public static long New(CoreEntities DB, long parentID, SysDictionary element)
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
                    DB.SysDictionaries.Add(element);
                    //Создаем привязку к родителю
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = element.ID });
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании элемента словаря.", e);
                }
            }
            return element.ID;
        }
        /// <summary>
        /// Удаляет элемент словаря
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор элемента словаря</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    SysDictionary element = DB.SysDictionaries.Find(id);
                    if (element != null)
                    {
                        /////////////////////////////////////////////////
                        // Нужно сделать проверку на использование елемента в других объектах
                        /////////////////////////////////////////////////

                        BaseObjectOperations.DelFromGroups(DB, id);
                        BaseObjectOperations.DelFromRoles(DB, id);
                        DB.SysDictionaries.Remove(element);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления элемента словаря.", e);
                }
            }
        }
        /// <summary>
        /// Возвращает список элементов группы
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="groupID">Идентификатор группы</param>
        /// <returns>Список элементов</returns>
        public static IQueryable<SysDictionary> GetItems(CoreEntities DB, long groupID)
        {
            return from og in DB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == groupID)
                   join d in DB.SysDictionaries.AsNoTracking() on og.ObjectID equals d.ID
                   select d;
        }
        public static IQueryable<SysDictionary> GetItems(CoreEntities DB, string guid)
        {
            return from g in DB.ObjGroups.Where(p => p.Code.Equals(guid))
                   join og in DB.ObjGroupObjects on g.ID equals og.GroupID
                   join d in DB.SysDictionaries on og.ObjectID equals d.ID
                   select d;
        }
    }
}