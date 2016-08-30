using BaseEntities;
using CoreAPI.Authorization;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CoreAPI.Extensions
{
    /// <summary>
    /// Пространство имен для реализации расширений
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Расширения для контекста БД
    /// </summary>
    public static class DbExtension
    {
        /// <summary>
        /// Возвращает доступные для чтения объекты
        /// </summary>
        /// <typeparam name="T">Тип сущности, по которой ограничивается доступ</typeparam>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список доступных объектов</returns>
        public static IQueryable<T> GetAllowObjects<T>(this DbContext db) where T : BaseEntity
        {
            var i = (HttpContext.Current.User as ICustomPrincipal);
            var q = from o in db.Set<T>().AsNoTracking()
                    where db.Set<SysObjectRole>().Any(p => p.ObjectID == o.ID && i.RoleIDs.Contains(p.RoleID))
                    select o;
            return q;
        }
        /// <summary>
        /// Возвращает доступные для чтения объекты.
        /// <note type="important">Применяется в случае расширенной структуры SysObjectRole в рамках контекста БД</note>
        /// </summary>
        /// <typeparam name="T">Тип сущности, по которой ограничивается доступ</typeparam>
        /// <typeparam name="E">Тип сущности-связки объекта с ролью (SysObjectRole)</typeparam>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список доступных объектов</returns>
        public static IQueryable<T> GetAllowObjects<T, E>(this DbContext db)
            where T : BaseEntity
            where E : SysObjectRole
        {
            var i = (HttpContext.Current.User as ICustomPrincipal);
            var q = from o in db.Set<T>().AsNoTracking()
                    where db.Set<E>().Any(p => p.ObjectID == o.ID && i.RoleIDs.Contains(p.RoleID))
                    select o;
            return q;
        }
    }
}
