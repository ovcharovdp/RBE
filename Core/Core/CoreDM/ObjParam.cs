
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    /// <summary>
    /// Параметр объекта
    /// </summary>
    public partial class ObjParam: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public ObjParam()
        {
            this.Items = new HashSet<ObjParamItem>();
            this.SetItems = new HashSet<ObjParamSetItem>();
            this.Values = new HashSet<ObjParamValue>();
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
    	/// Тип
    	/// </summary>
        public string Type { get; set; }
    	/// <summary>
    	/// Код
    	/// </summary>
        public string Code { get; set; }
    	/// <summary>
    	/// Максимальное количество значений
    	/// </summary>
        public byte MaxCount { get; set; }
    	/// <summary>
    	/// Максимальная длина значения
    	/// </summary>
        public short Length { get; set; }
    	/// <summary>
    	/// Точность
    	/// </summary>
        public byte Precision { get; set; }
    	/// <summary>
    	/// Порядок (учитывается при отображении полей ввода)
    	/// </summary>
        public byte Order { get; set; }
    	/// <summary>
    	/// Способ выбора значения
    	/// </summary>
        public string UI { get; set; }
    
    	/// <summary>
    	/// Допустимые значения параметра
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<ObjParamItem> Items { get; set; }
    	/// <summary>
    	/// Вложенные параметры
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<ObjParamSetItem> SetItems { get; set; }
    	/// <summary>
    	/// Значения параметра
    	/// </summary>
    	[JsonIgnore]
        public virtual ICollection<ObjParamValue> Values { get; set; }
    }
}
