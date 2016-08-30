namespace BaseEntities
{
    using System.Collections.Generic;
    using BaseEntities;
    using Newtonsoft.Json;

    /// <summary>
    /// Состояние объекта
    /// </summary>
    public partial class ObjCatalogState : BaseEntity
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ObjCatalogState()
        {
            this.Rules = new HashSet<ObjCatalogRule>();
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Css стиль объекта
        /// </summary>
        public string CssStyle { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Журнал
        /// </summary>
        public virtual ObjCatalog Catalog { get; set; }
        /// <summary>
        /// Правила изменения состояния
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ObjCatalogRule> Rules { get; set; }
    }
}