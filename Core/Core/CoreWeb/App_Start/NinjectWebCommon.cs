namespace CoreWeb.App_Start
{
    using System;
    using System.Web;
    using CoreWeb.Models.StartPage;
    using Ninject;
    using Ninject.Web.Common;
    using CoreAPI.Types;
    using CoreWeb.Providers;
    using CoreAPI.ChangeHistory;
    /// <summary>
    /// Класс, реализующий DI
    /// </summary>
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Ninject.Web.WebApi.NinjectDependencyResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // идентификатор меню может быть перенесен в параметр "MenuRootGID" файла конфикурации
            kernel.Bind<IMainMenuLoader>().To<MainMenuLoader>().InRequestScope().WithConstructorArgument("gid", "AAAF3133-6783-45E5-A2AC-3EC470F5671A");
            kernel.Bind<ICoreDBContext>().To<DBProvider>().InRequestScope();
            kernel.Bind<IWorkItemLoader>().To<BaseWorkItemLoader>().InRequestScope();
        }
    }
}
