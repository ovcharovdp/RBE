using BaseEntities;
using System.Collections.Generic;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Модель журнала объектов с документооборотом
    /// </summary>
    public class DocFlowModel : CatalogModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="columnLoader">Загрузчик полей</param>
        public DocFlowModel(IColumnLoader columnLoader) : base(columnLoader) { _columnLoader = columnLoader; }
        /// <summary>
        /// Список css-стилей для записей таблицы, завсящий от поля состояния
        /// </summary>
        public List<ObjCatalogState> States { get; set; }
    }
}
