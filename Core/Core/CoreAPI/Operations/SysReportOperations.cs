using BaseEntities;
using CoreDM;
using System;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Отчет"
    /// </summary>
    public class SysReportOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="element">Отчет</param>
        public static void Check(SysReport element)
        {
            if (element == null)
                throw new Exception("Элемент не определен.");
            if (string.IsNullOrEmpty(element.Name.Trim()))
                throw new Exception("Не задано название отчета.");
            if (string.IsNullOrEmpty(element.Path.Trim()))
                throw new Exception("Не задан путь к отчету.");
            if (string.IsNullOrEmpty(element.Path.Trim()))
                throw new Exception("Не задана ссылка на сервер отчетов.");
        }
        /// <summary>
        /// Создает новый отчет
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <param name="element">Отчет</param>
        /// <returns>Идентификатор отчета</returns>
        public static long New(CoreEntities DB, long parentID, SysReport element)
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
                    DB.SysReports.Add(element);
                    //Создаем привязку к родителю
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = element.ID });
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании отчета.", e);
                }
            }
            return element.ID;
        }
        /// <summary>
        /// Удаляет отчет
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор отчета</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    SysReport element = DB.SysReports.Find(id);
                    if (element != null)
                    {
                        BaseObjectOperations.DelFromGroups(DB, id);
                        BaseObjectOperations.DelFromRoles(DB, id);
                        DB.SysReports.Remove(element);
                        DB.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления отчета.", e);
                }
            }
        }
    }
}
