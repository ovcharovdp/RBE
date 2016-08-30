namespace BaseEntities
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Журнал системы
    /// </summary>
    public partial class ObjCatalog: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public ObjCatalog()
        {
            this.Fields = new HashSet<ObjCatalogField>();
            this.States = new HashSet<ObjCatalogState>();
        }
    
    	/// <summary>
    	/// Глобальный идентификатор журнала
    	/// </summary>
        public string GID { get; set; }
    	/// <summary>
    	/// Наименование (заголовок)
    	/// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    	/// <summary>
    	/// Идентификатор корневой группы элементов группового журнала
    	/// </summary>
        public Nullable<long> RootID { get; set; }
    
    	/// <summary>
    	/// Поля журнала
    	/// </summary>
        [JsonIgnore]
        public virtual ICollection<ObjCatalogField> Fields { get; set; }
    	/// <summary>
    	/// Состояния объекта
    	/// </summary>
        [JsonIgnore]
        public virtual ICollection<ObjCatalogState> States { get; set; }
    }
}
