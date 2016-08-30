using CoreDM;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Модель журнала объектов, привязанных к группам
    /// </summary>
    public class GroupedListModel : CatalogModel
    {
        private long _groupID;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="columnLoader">Загрузчик полей</param>
        /// <param name="groupID">Идентификатор группы</param>
        public GroupedListModel(IColumnLoader columnLoader, long groupID) : base(columnLoader) { _groupID = groupID; }
        /// <summary>
        /// Идентификатор группы
        /// </summary>
        public long GroupID { get { return _groupID; } }
    }
}