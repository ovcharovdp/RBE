using Newtonsoft.Json;

namespace BaseEntities
{
    /// <summary>
    /// Поля журналов
    /// </summary>
    public partial class ObjCatalogField : BaseEntity
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ObjCatalogField()
        {
            this.Width = 0;
            this.Filterable = false;
            this.Sortable = false;
            this.Detailed = false;
        }

        /// <summary>
        /// Наименование (заголовок) поля
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код. Должен совпадать с полем класса, возвращаемого с сервера
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Максимальная длина значения
        /// </summary>
        public short Width { get; set; }
        /// <summary>
        /// Признак возможности установки фильтра
        /// </summary>
        public bool Filterable { get; set; }
        /// <summary>
        /// Признак сортировки
        /// </summary>
        public bool Sortable { get; set; }
        /// <summary>
        /// Формат отображения значения
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// Порядок поля в журнале
        /// </summary>
        public byte Order { get; set; }
        /// <summary>
        /// Способ выбора значения
        /// </summary>
        public string UI { get; set; }
        /// <summary>
        /// Шаблон отображения значений
        /// </summary>
        public string Template { get; set; }
        /// <summary>
        /// Признак вывода поля в панель "Подробно"
        /// </summary>
        public bool Detailed { get; set; }
        /// <summary>
        /// Признак экспорта в Excel
        /// </summary>
        public bool Exportable { get; set; }

        /// <summary>
        /// Журнал
        /// </summary>
        [JsonIgnore]
        public virtual ObjCatalog Catalog { get; set; }
    }
}
