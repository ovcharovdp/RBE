using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Модель журнала объектов
    /// </summary>
    public class CatalogModel
    {
        /// <summary>
        /// Загрузчик полей журнала
        /// </summary>
        protected IColumnLoader _columnLoader;
        /// <summary>
        /// Название класса контроллера
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// Заголовок страницы браузера
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Тип сущности для получения списка объектов по протоколу OData
        /// </summary>
        public string ODataEntity { get; set; }
        /// <summary>
        /// Сокращенное имя класса контроллера (без окончания Controller)
        /// </summary>
        public string ControllerShortName { get { return Controller.Replace("Controller", string.Empty); } }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="columnLoader">Загрузчик полей</param>
        public CatalogModel(IColumnLoader columnLoader) { _columnLoader = columnLoader; }
        /// <summary>
        /// Список полей журнала
        /// </summary>
        public List<ColumnModel> Columns { get { return _columnLoader == null ? null : _columnLoader.Columns; } }
        string _expandData;
        /// <summary>
        /// Возвращает строку для прогрузки вложенных блоков данных
        /// </summary>
        public string ExpandEntity
        {
            get
            {
                if (string.IsNullOrEmpty(_expandData))
                {
                    var q = Columns.Where(p => p.Name.Contains('.')).Select(p => p.Name.Substring(0, p.Name.LastIndexOf('.')).Replace('.', '/')).Distinct();
                    _expandData = string.Join(",", q);
                }
                return _expandData;
            }
        }
    }
}
