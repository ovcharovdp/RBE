[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CoreWeb.App_Start.AppInitializer), "Init")]

namespace CoreWeb.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject.Web.Common;
    /// <summary>
    /// Реализация базовой инициализации приложения
    /// </summary>
    public static class AppInitializer
    {
        /// <summary>
        /// Инициализация.
        /// <para>Выполняется регистрация модулей <code>OnePerRequestHttpModule, NinjectHttpModule</code></para>
        /// </summary>
        public static void Init()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
        }
    }
}