
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    /// <summary>
    /// Пункт меню
    /// </summary>
    public partial class SysMenu: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public SysMenu()
        {
            this.Childs = new HashSet<SysMenu>();
        }
    
    	/// <summary>
    	/// Наименование
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Описание
    	/// </summary>
        public string Description { get; set; }
    	/// <summary>
    	/// Имя файла пиктограммы, используемой при отображении
    	/// </summary>
        public string ImageName { get; set; }
    	/// <summary>
    	/// Родительский пункт меню
    	/// </summary>
        public Nullable<long> ParentID { get; set; }
    	/// <summary>
    	/// Команда
    	/// </summary>
        public string Url { get; set; }
    	/// <summary>
    	/// Параметры команды
    	/// </summary>
        public string Params { get; set; }
    	/// <summary>
    	/// Тип меню
    	/// </summary>
        public string Type { get; set; }
    	/// <summary>
    	/// Порядок (для сортировки)
    	/// </summary>
        public byte Order { get; set; }
    	/// <summary>
    	/// Признак отображения в виде плитки
    	/// </summary>
        public bool IsVisible { get; set; }
    
    	/// <summary>
    	/// Дочерние пункты меню
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<SysMenu> Childs { get; set; }
    	/// <summary>
    	/// Родительский пункт меню
    	/// </summary>
        public virtual SysMenu Parent { get; set; }
    }
}
