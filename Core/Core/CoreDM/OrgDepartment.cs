
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    /// <summary>
    /// Организационная единица
    /// </summary>
    public partial class OrgDepartment: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public OrgDepartment()
        {
            this.Childs = new HashSet<OrgDepartment>();
        }
    
    	/// <summary>
    	/// Идентификатор вышестоящей организации
    	/// </summary>
        public Nullable<long> ParentID { get; set; }
    	/// <summary>
    	/// Наименование
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Идентификатор типа
    	/// </summary>
        public long TypeID { get; set; }
    	/// <summary>
    	/// Полное имя
    	/// </summary>
        public string FullName { get; set; }
    	/// <summary>
    	/// Структурное имя
    	/// </summary>
        public string StructName { get; set; }
    	/// <summary>
    	/// Короткое имя (обозначение)
    	/// </summary>
        public string ShortName { get; set; }
        public string Code { get; set; }
        public string FactAddress { get; set; }
        public string LegalAddress { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }

        /// <summary>
        /// Подчиненные организации
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<OrgDepartment> Childs { get; set; }
    	/// <summary>
    	/// Вышестоящая организация
    	/// </summary>
        public virtual OrgDepartment Parent { get; set; }
    	/// <summary>
    	/// Тип
    	/// </summary>
        public virtual SysDictionary Type { get; set; }
    }
}
