using CoreDM;
using System;

namespace CoreAPI.Components
{
    /// <summary>
    /// Пространство имен для реализации операций, необходимых при работе с пользовательскими компонентами.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс для описания действий элементов управления, отражающих параметры объекта
    /// </summary>
    public class Editor
    {
        /// <summary>
        /// Возвращает значение параметра в зависимости от его типа
        /// </summary>
        /// <param name="param">Параметр</param>
        /// <param name="value">Значение параметра</param>
        /// <returns>Значение</returns>
        public static object GetValue(ObjParam param, ObjParamValue value)
        {
            switch (param.Type)
            {
                case "DATE":
                    {
                        return value.DateValue;
                    }
                case "NUMBER":
                    {
                        return value.NumberValue;
                    }
                default:
                    return value.VarcharValue;
            }
        }
        /// <summary>
        /// Формирует имя пользовательского элемента управления для параметра
        /// </summary>
        /// <param name="parentID">Объект-владелец</param>
        /// <param name="id">Идентификатор параметра</param>
        /// <param name="order">Порядок</param>
        /// <returns>Имя элемента управления</returns>
        public static string GetName(long parentID, long id, long order)
        {
            if (id < 0)
                return string.Format("f{0}_c{1}_{2}", parentID, Math.Abs(id), order);
            else
                return string.Format("f{0}_{1}_{2}", parentID, id, order);
        }
    }
}