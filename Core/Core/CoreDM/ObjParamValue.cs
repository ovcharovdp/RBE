
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;
    
    /// <summary>
    /// Значения параметра объекта
    /// </summary>
    public partial class ObjParamValue: BaseEntity
    {
    	/// <summary>
    	/// Идентификатор объекта
    	/// </summary>
        public long ParentID { get; set; }
    	/// <summary>
    	/// Идентификатор параметра
    	/// </summary>
        public long ParamID { get; set; }
    	/// <summary>
    	/// Порядок
    	/// </summary>
        public byte Order { get; set; }
    	/// <summary>
    	/// Строковое значение
    	/// </summary>
        public string VarcharValue { get; set; }
    	/// <summary>
    	/// Числовое значение
    	/// </summary>
        public Nullable<decimal> NumberValue { get; set; }
    	/// <summary>
    	/// Значение типа дата
    	/// </summary>
        public Nullable<System.DateTime> DateValue { get; set; }
    	/// <summary>
    	/// Значение-объект
    	/// </summary>
        public Nullable<long> ObjectValue { get; set; }
    
    	/// <summary>
    	/// Параметр
    	/// </summary>
        public virtual ObjParam Param { get; set; }
    }
}
