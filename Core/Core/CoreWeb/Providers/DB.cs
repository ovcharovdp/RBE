using System.Web;
using CoreDM;
using CoreAPI.Types;

namespace CoreWeb.Providers
{
    //internal static class RecursiveExtension
    //{
    //    public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursiveFunc)
    //    {
    //        if (source != null)
    //        {
    //            foreach (var mainItem in source)
    //            {
    //                yield return mainItem;

    //                IEnumerable<T> recursiveSequence = (recursiveFunc(mainItem) ?? new T[] { }).SelectRecursive(recursiveFunc);

    //                if (recursiveSequence != null)
    //                {
    //                    foreach (var recursiveItem in recursiveSequence)
    //                    {
    //                        yield return recursiveItem;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //public static class RecursiveExtension
    //{
    //    public static IQueryable<T> TestAccess<T>(this DbContext source) where T : SysDictionary
    //    {
    //        //if (source != null)
    //        //{
    //        //    foreach (var mainItem in source)
    //        //    {
    //        //        yield return mainItem;

    //        //        IEnumerable<T> recursiveSequence = (recursiveFunc(mainItem) ?? new T[] { }).SelectRecursive(recursiveFunc);

    //        //        if (recursiveSequence != null)
    //        //        {
    //        //            foreach (var recursiveItem in recursiveSequence)
    //        //            {
    //        //                yield return recursiveItem;
    //        //            }
    //        //        }
    //        //    }
    //        //}
    //        var q = source.Set<T>().AsNoTracking().Where(p => p.ID == 934);
    //        return q;
    //    }
    //}
    /// <summary>
    /// Пространство имен для реализации источников данных в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Источники данных
    /// </summary>
    public class DBProvider : ICoreDBContext
    {
        const string CoreDMContextKey = "CoreDMEntities";
        /// <summary>
        /// Источник данных сущностей системы
        /// </summary>
        public CoreEntities CoreEntities
        {
            get
            {
                if (HttpContext.Current.Items[CoreDMContextKey] == null)
                    HttpContext.Current.Items[CoreDMContextKey] = new CoreEntities();
                return (CoreEntities)HttpContext.Current.Items[CoreDMContextKey];
            }
        }
    }
}