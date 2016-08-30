
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Допустимые значения параметра
    /// </summary>
    public partial class ObjParamItem
    {
    	/// <summary>
    	/// Идентификатор параметра
    	/// </summary>
        public long ParamID { get; set; }
    	/// <summary>
    	/// Значение
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Порядок
    	/// </summary>
        public byte Order { get; set; }
    	/// <summary>
    	/// Надпись значения (используется для отображения)
    	/// </summary>
        public string Label { get; set; }
    
    	/// <summary>
    	/// Параметр, к которому относится значение
    	/// </summary>
        public virtual ObjParam Param { get; set; }
    }
}
