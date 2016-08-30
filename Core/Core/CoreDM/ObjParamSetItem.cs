
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Подчиненные параметры
    /// </summary>
    public partial class ObjParamSetItem
    {
    	/// <summary>
    	/// Идентификатор родительского параметра
    	/// </summary>
        public long ParentID { get; set; }
    	/// <summary>
    	/// Идентификатор подчиненного параметра
    	/// </summary>
        public long ParamID { get; set; }
    	/// <summary>
    	/// Порядок
    	/// </summary>
        public byte Order { get; set; }
    
    	/// <summary>
    	/// Параметр набора
    	/// </summary>
        public virtual ObjParam Param { get; set; }
    }
}
