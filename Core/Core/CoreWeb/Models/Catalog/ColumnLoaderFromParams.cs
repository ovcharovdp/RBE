using CoreDM;
using System.Linq;

namespace CoreWeb.Models.Catalog
{
    /// <summary>
    /// Класс, реализующий загрузку полей на основе сущности "Параметр объекта"
    /// </summary>
    public class ColumnLoaderFromParams : BaseColumnLoader
    {
        private long _groupID;
        /// <summary>
        /// Идентификатор группы, содержащей параметры
        /// </summary>
        protected long GroupID { get { return _groupID; } }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="groupID">Идентификатор группы, содержащей настройки полей</param>
        public ColumnLoaderFromParams(CoreEntities db, long groupID) : base(db) { _groupID = groupID; }
        /// <inheritdoc />
        protected override void LoadColumns()
        {
            var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == _groupID)
                    join p in _db.ObjParams.AsNoTracking() on g.ObjectID equals p.ID
                    orderby p.Order
                    select new ColumnModel() { Name = p.Code, Title = p.Name, Type = p.Type };
            _columns = q.ToList();
        }
    }
}