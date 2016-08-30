using System.Data.Entity;
using System.Linq;

namespace CoreAPI.Operations
{
    /// <summary>
    /// Пространство имен для реализации логики работы с сущностями БД
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Базовый класс для реализации действий с сущностями БД
    /// </summary>
    public class BaseDBOperations
    {
        const string PG_NEXT_ID_CMD = "select nextval('object_ids')";
        const string ORA_NEXT_ID_CMD = "select object_ids.nextval from dual";
        /// <summary>
        /// Возвращает следующий идентификатор для присвоения создаваемому объекту
        /// <para><note type="note">Идентификатор формируется посредством получения следующего элемента последовательности <b>OBJECT_IDS</b> СУБД Oracle.</note></para>
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <returns>Идентификатор объекта</returns>
        protected static long GetNextID(DbContext db)
        {
            string t = db.Database.Connection.GetType().Namespace;
            return db.Database.SqlQuery<long>(t.Equals("Npgsql") ? PG_NEXT_ID_CMD : ORA_NEXT_ID_CMD, new object[] { }).FirstOrDefault();
        }
    }
}
