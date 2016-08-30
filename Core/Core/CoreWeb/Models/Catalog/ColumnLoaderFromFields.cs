using BaseEntities;
using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Класс, реализующий загрузку полей на основе сущности "Поле журнала"
    /// </summary>
    public class ColumnLoaderFromFields : BaseColumnLoader
    {
        IList<ObjCatalogField> _fields;
        /// <summary>
        /// Журнал системы
        /// </summary>
        protected ObjCatalog _catalog;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="gid">Глобальный идентификатор журнала</param>
        public ColumnLoaderFromFields(CoreEntities db, string gid)
            : base(db)
        {
            _catalog = db.ObjCataloges.Include("Fields").Include("States").FirstOrDefault(p => p.GID.Equals(gid));
            _fields = _catalog.Fields.ToList();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="catalog">Журнал системы</param>
        public ColumnLoaderFromFields(CoreEntities db, ObjCatalog catalog)
            : base(db)
        {
            _catalog = catalog;
            _fields = _catalog.Fields.ToList();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="fields">Поля журнала</param>
        public ColumnLoaderFromFields(CoreEntities db, ICollection<ObjCatalogField> fields)
            : base(db)
        {
            _fields = fields.ToList();
        }
        /// <inheritdoc />
        protected override void LoadColumns()
        {
            _columns = _fields.OrderBy(p => p.Order).Select(p => new ColumnModel()
            {
                Name = p.Code,
                Title = p.Name,
                Type = p.Type,
                Filterable = p.Filterable,
                Sortable = p.Sortable,
                Width = p.Width,
                Format = p.Format,
                UI = p.UI,
                Template = p.Template,
                Detailed = p.Detailed,
                Exportable = p.Exportable
            }).ToList();
        }
    }
}