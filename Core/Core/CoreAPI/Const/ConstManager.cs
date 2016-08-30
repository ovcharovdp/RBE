using CoreAPI.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreAPI.Const
{
    /// <summary>
    /// Пространство имен для реализации операций работы с константами.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    class ConstValue
    {
        public string GID { get; set; }
        public long Value { get; set; }
    }
    /// <summary>
    /// Реализует логику получения констант, в качестве которых выступают жестко заданные
    /// ссылки на объекты системы.
    /// </summary>
    public static class ConstManager
    {
        static IDictionary<string, long> _consts = new Dictionary<string, long>();
        /// <summary>
        /// Получает значение константы.
        /// </summary>
        /// <param name="gid">Глобальный идентификатор константы</param>
        /// <param name="db">Менеджер контекстов БД</param>
        /// <returns>Идентификатор объекта</returns>
        public static long Get(string gid, ICoreDBContext db)
        {
            if (!_consts.ContainsKey(gid))
            {
                var c = db.CoreEntities.SysConsts.AsNoTracking().FirstOrDefault(p => p.GID.Equals(gid));
                if (c == null)
                    throw new ArgumentOutOfRangeException("Константа не найдена.");
                _consts.Add(gid, c.Value);
                return c.Value;
            }
            return _consts[gid];
        }
        /// <summary>
        /// Получает значение константы. Если оно отсутствует, то загрузка происходит 
        /// с помощью переданного в качестве второго аргумента загрузчика.
        /// </summary>
        /// <param name="gid">Глобальный идентификатор константы</param>
        /// <param name="loader">Загрузчик значения константы</param>
        /// <returns>Идентификатор объекта</returns>
        public static long Get(string gid, IConstLoader loader)
        {
            if (!_consts.ContainsKey(gid))
            {
                long c = loader.Load(gid);
                if (c <= 0)
                    throw new ArgumentException("Не удалось определить значение константы " + gid);
                _consts.Add(gid, c);
            }
            return _consts[gid];
        }
    }
}
