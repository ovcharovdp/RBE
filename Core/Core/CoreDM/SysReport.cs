
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;
    
    /// <summary>
    /// Отчеты
    /// </summary>
    public partial class SysReport: BaseEntity
    {
    	/// <summary>
    	/// Наименование
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>
    	/// Описание
    	/// </summary>
        public string Description { get; set; }
    	/// <summary>
    	/// Путь к отчету на сервере
    	/// </summary>
        public string Path { get; set; }
    	/// <summary>
    	/// Ссылка на сервер отчетов
    	/// </summary>
        public string Url { get; set; }
    }
}
