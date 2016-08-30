using System.Collections.Generic;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Интерфейс, описывающий загрузчик полей журнала
    /// </summary>
    public interface IColumnLoader
    {
        /// <summary>
        /// Список полей журнала
        /// </summary>
        List<ColumnModel> Columns { get; }
    }
}
