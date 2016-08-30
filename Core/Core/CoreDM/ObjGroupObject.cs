
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Связка объекта с группой
    /// </summary>
    public partial class ObjGroupObject
    {
    	/// <summary>
    	/// Идентификатор группы
    	/// </summary>
        public long GroupID { get; set; }
    	/// <summary>
    	/// Идентификатор объекта
    	/// </summary>
        public long ObjectID { get; set; }
    }
}
