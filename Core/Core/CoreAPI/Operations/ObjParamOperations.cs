using BaseEntities;
using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Параметр объекта"
    /// </summary>
    public class ObjParamOperations : BaseDBOperations
    {
        /// <summary>
        /// Проверяет корректность заполнения данных
        /// </summary>
        /// <param name="param">Параметр</param>
        public static void Check(ObjParam param)
        {
            if (param == null)
                throw new Exception("Параметр не определен.");
            if (string.IsNullOrEmpty(param.Name))
                throw new Exception("Не задано название параметра.");
            if (string.IsNullOrEmpty(param.Type))
                throw new Exception("Не задан тип значения параметра.");
        }
        /// <summary>
        /// Создает новый параметр
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Идентификатор родительской группы</param>
        /// <param name="param">Параметр</param>
        /// <returns>Идентификатор параметра</returns>
        public static long New(CoreEntities DB, long parentID, ObjParam param)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (parentID <= 0)
                        throw new Exception("Не задана родительская группа.");
                    Check(param);

                    if (param.ID == 0)
                        param.ID = GetNextID(DB);
                    DB.ObjParams.Add(param);
                    //Создаем привязку к родителю
                    DB.ObjGroupObjects.Add(new ObjGroupObject() { GroupID = parentID, ObjectID = param.ID });
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка при создании параметра.", e);
                }
            }
            return param.ID;
        }
        /// <summary>
        /// Удаляет список возможных значений параметра
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор параметра</param>
        public static void ClearItems(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var q = DB.ObjParamItems.Where(p => p.ParamID == id);
                    foreach (var r in q)
                    {
                        DB.ObjParamItems.Remove(r);
                    }
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления списка значений.", e);
                }
            }
        }
        /// <summary>
        /// Удаляет параметр
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="id">Идентификатор параметра</param>
        public static void Del(CoreEntities DB, long id)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    ObjParam param = DB.ObjParams.Find(id);
                    if (param == null) return;

                    //получаем привязки - дочерние группы
                    if (DB.ObjParamValues.Any(p => p.ParamID == id))
                        throw new Exception("Существуют значения по данному параметру. Удаление невозможно.");

                    BaseObjectOperations.DelFromGroups(DB, id);
                    DB.ObjParams.Remove(param);
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка удаления параметра.", e);
                }
            }
        }
    }
}