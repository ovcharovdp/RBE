using CoreWeb.Attributes.MVC;
using System.Web.Mvc;

namespace CoreWeb
{
    /// <summary>
    /// Реализация конфигуратора фильтров
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Регистрация фильтров
        /// </summary>
        /// <param name="filters">Фильтры</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAllErrorAttribute());
            filters.Add(new ExtAuthorizeAttribute() { AdminOnly = false });
        }
    }
}