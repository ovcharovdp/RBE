using BaseEntities;
using CoreAPI.Types;
using CoreDM;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Класс, реализующий загрузку полей на основе сущности "Поле журнала"
    /// </summary>
    public class DocFlowColumnLoader : ColumnLoaderFromFields
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="catalog">Журнал системы</param>
        public DocFlowColumnLoader(CoreEntities db, ObjCatalog catalog) : base(db, catalog) { }
        /// <inheritdoc />
        protected override void LoadColumns()
        {
            base.LoadColumns();
            if (_columns.Count(p => p.Name.Equals("StateID")) == 0)
                this._columns.Add(new ColumnModel()
                {
                    Name = "State.ID",
                    Title = "Состояние",
                    Type = "OBJECT",
                    Filterable = true,
                    Sortable = true,
                    Width = 150,
                    Format = "",
                    UI = "",
                    Template = "",
                    Detailed = false,
                    Exportable = false,
                    Values = _catalog.States.Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToList()
                });
        }
    }
}