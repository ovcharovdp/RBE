using CoreDM;
using System;
using System.Linq;
using System.Transactions;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Класс, реализующий действия над сущностью "Значение параметра"
    /// </summary>
    public class ObjParamValueOperations : BaseDBOperations
    {
        /// <summary>
        /// Создает новое значение параметра
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="value">Значение</param>
        /// <exception cref="ArgumentException">При отсутствии в системе параметра с заданным идентификатором</exception>
        /// <returns>Идентификатор созданного объекта</returns>
        public static long New(CoreEntities DB, ObjParamValue value)
        {
            if (DB.ObjParams.Find(value.ParamID) == null)
                throw new ArgumentException(string.Format("Не найден параметр (ID={0}).", value.ParamID));
            value.ID = GetNextID(DB);
            DB.ObjParamValues.Add(value);
            DB.SaveChanges();
            return value.ID;
        }
        /// <summary>
        /// Удаляет значение параметра
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="value">Значение параметра</param>
        public static void Del(CoreEntities DB, ObjParamValue value)
        {
            if (value == null) return;
            string _type = DB.ObjParams.AsNoTracking().SingleOrDefault(p => p.ID == value.ParamID).Type;
            if ("SET".Equals(_type))
                ClearValues(DB, value.ID);
            DB.ObjParamValues.Remove(value);
            DB.SaveChanges();
        }
        /// <summary>
        /// Удаляет значение параметра
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Объект, которому принадлежит значение</param>
        /// <param name="id">Идентификатор параметра</param>
        /// <param name="order">Порядок</param>
        public static void Del(CoreEntities DB, long parentID, long id, short order)
        {
            ObjParamValue value = DB.ObjParamValues.Find(new object[] { parentID, id, order });
            if (value != null)
                Del(DB, value);
        }
        /// <summary>
        /// Изменяет значение параметра объекта
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="parentID">Объект, которому принадлежит значение</param>
        /// <param name="id">Идентификатор параметра</param>
        /// <param name="order">Порядок</param>
        /// <param name="value">Новое значение</param>
        public static void Resolve(CoreEntities DB, long parentID, long id, byte order, object value)
        {
            try
            {
                ObjParam param = DB.ObjParams.AsNoTracking().FirstOrDefault(p => p.ID == id);
                ObjParamValue Value = DB.ObjParamValues.Find(new object[] { parentID, id, order });
                if (Value == null)
                {
                    Value = new ObjParamValue() { ID = GetNextID(DB), ParamID = id, ParentID = parentID, Order = order };
                    DB.ObjParamValues.Add(Value);
                }
                switch (param.Type)
                {
                    case "NUMBER":
                        Value.NumberValue = Convert.ToDecimal(value);
                        break;
                    case "OBJECT":
                        Value.ObjectValue = Convert.ToInt64(value);
                        break;
                    case "TEXT":
                        Value.VarcharValue = (string)value;
                        break;
                    case "DATE":
                        Value.DateValue = (value is DateTime) ? (DateTime)value : DateTime.Parse((string)value);
                        break;
                }
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Ошибка сохранения значения параметра (ID:{0};order:{1}).", id, order), e);
            }
        }
        /// <summary>
        /// Удаляет значения параметров объекта
        /// </summary>
        /// <param name="DB">Контекст БД</param>
        /// <param name="objectID">Идентификатор объекта</param>
        public static void ClearValues(CoreEntities DB, long objectID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var paramList = DB.ObjParamValues.Where(p => p.ParentID == objectID).Select(p => p);
                    foreach (var r in paramList)
                    {
                        Del(DB, r);
                    }
                    DB.SaveChanges();
                    scope.Complete();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}