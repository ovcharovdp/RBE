using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Пространство имен для реализации загрузки полей журнала
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Базовый класс для реализации загрузчика полей журнала
    /// </summary>
    public abstract class BaseColumnLoader : IColumnLoader
    {
        /// <summary>
        /// Контекст БД
        /// </summary>
        protected CoreEntities _db;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        public BaseColumnLoader(CoreEntities db) { _db = db; }
        /// <summary>
        /// Список полей журнала
        /// </summary>
        protected List<ColumnModel> _columns;
        /// <summary>
        /// Формирует список полей
        /// </summary>
        protected abstract void LoadColumns();
        /// <summary>
        /// Возвращает список полей журнала
        /// </summary>
        public List<ColumnModel> Columns
        {
            get
            {
                if (_columns == null) LoadColumns();
                return _columns;
            }
        }
    }
}