using CoreAPI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Класс, реализующий действия над полем журнала
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ColumnModel()
        {
            Filterable = true;
            Sortable = true;
        }
        private string _type;
        /// <summary>
        /// Наименование поля (должно совпадать с полем класса, значение которого представляет)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Заголовок поля
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Признак фильтрации поля
        /// </summary>
        public bool Filterable { get; set; }
        /// <summary>
        /// Признак сортировки поля
        /// </summary>
        public bool Sortable { get; set; }
        /// <summary>
        /// Тип поля
        /// </summary>
        public string Type
        {
            get
            {
                switch (_type)
                {
                    case "DATE": return "date";
                    case "OBJECT": return "number";
                    case "NUMBER": return "number";
                    case "LOGIC": return "boolean";
                    default: return "string";
                }
            }
            set { _type = value; }
        }
        /// <summary>
        /// Ширина поля
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Список доступных значений
        /// </summary>
        public List<BaseObject> Values { get; set; }
        /// <summary>
        /// Формат отображения данных
        /// </summary>
        public string Format { get; set; }
        ///<summary>
        ///Способ выбора значения
        ///</summary>
        public string UI { get; set; }
        ///<summary>
        ///Шаблон отображения значений
        ///</summary>
        public string Template { get; set; }
        /// <summary>
        ///Признак отображения поля в области "Подробно"
        /// </summary>
        public bool Detailed { get; set; }
        /// <summary>
        ///Признак экспорта поля в Excel
        /// </summary>
        public bool Exportable { get; set; }
    }
}