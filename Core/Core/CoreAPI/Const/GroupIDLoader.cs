using CoreDM;
using System;
using System.Linq;

namespace CoreAPI.Const
{
    /// <summary>
    /// Реализует загрузку идентификатора группы
    /// </summary>
    public class GroupIDLoader : IConstLoader
    {
        CoreEntities _db;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Контекст БД</param>
        public GroupIDLoader(CoreEntities db) { _db = db; }
        /// <inheritdoc />
        public long Load(string gid)
        {
            try
            {
                var q = _db.ObjGroups.AsNoTracking().Single(p => p.Code.Equals(gid));
                return q.ID;
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка получения значения константы.", e);
            }
        }
    }
}
