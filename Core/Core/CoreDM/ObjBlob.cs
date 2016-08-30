
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Данные объекта (файлы картинок и т.п.)
    /// </summary>
    public partial class ObjBlob
    {
    	/// <summary>
    	/// Идентификатор объекта
    	/// </summary>
        public long ParentID { get; set; }
    	/// <summary>
    	/// Данные
    	/// </summary>
        public byte[] Data { get; set; }
    	/// <summary>
    	/// Порядок
    	/// </summary>
        public byte Order { get; set; }
    }
}
