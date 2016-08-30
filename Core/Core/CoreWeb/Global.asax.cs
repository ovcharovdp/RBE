using CoreWeb.App_Start;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using System.Web.SessionState;
using CoreWeb.Authorization;
using System.Web.Security;
using CoreDM;
using CoreAPI.Types;

namespace CoreWeb
{
    /// <summary>
    /// Пространство имен для Web-ядра.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <inheritdoc />
    public class MvcApplication : System.Web.HttpApplication
    {
        private CoreEntities coreDB
        {
            get
            {
                return DependencyResolver.Current.GetService<ICoreDBContext>().CoreEntities;
            }
        }
        private AuthenticationMode modeAuth = (WebConfigurationManager.OpenWebConfiguration("~").GetSection("system.web/authentication") as AuthenticationSection).Mode;
        /// <summary>
        /// Начальная точка выполнения приложения
        /// </summary>
        protected void Application_Start()
        {
            NinjectWebCommon.Start();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        /// <summary>
        /// Реализация события при завершении работы приложения
        /// </summary>
        protected void Application_End()
        {
            NinjectWebCommon.Stop();
        }
        /// <summary>
        /// Действие после авторизации пользователя в системе
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/odata"))
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
        void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    if (modeAuth == AuthenticationMode.Windows)
                    {
                        if (HttpContext.Current.User.Identity is System.Security.Principal.WindowsIdentity)
                            if (this.Context.Session != null)
                            {
                                if (HttpContext.Current.Session["user"] != null)
                                {
                                    HttpContext.Current.User = this.Context.Session["user"] as CustomPrincipal;
                                }
                                else
                                {
                                    GetCustomPrincipalWindow(HttpContext.Current.User.Identity.Name);
                                }
                            }
                    }
                    else if (modeAuth == AuthenticationMode.Forms)
                    {
                        if (this.Context.Session != null)
                        {
                            if (HttpContext.Current.Session["user"] != null)
                            {
                                HttpContext.Current.User = this.Context.Session["user"] as CustomPrincipal;
                            }
                            else
                            {
                                GetCustomPrincipalForms(HttpContext.Current.User.Identity.Name);
                            }
                        }
                    }
        }
        private void GetCustomPrincipalWindow(string name)
        {
            System.DirectoryServices.AccountManagement.UserPrincipal principal = null;
            using (var context = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain))
            {
                try
                {
                    principal = System.DirectoryServices.AccountManagement.UserPrincipal.FindByIdentity(context, name);
                }
                catch { }
            }
            CustomPrincipal _customPrincipal = new CustomPrincipal(coreDB, User, principal);
            if (principal != null)
            {
                _customPrincipal.FirstName = principal.GivenName;
                _customPrincipal.LastName = principal.Surname;
            }
            else
            {
                _customPrincipal.FirstName = name;
            }
            //newUser.isAdmin = System.Web.Security.Roles.IsUserInRole(name, "Администратор");
            this.Context.Session["user"] = HttpContext.Current.User = _customPrincipal;
        }

        private void GetCustomPrincipalForms(string name)
        {
            System.Security.Principal.IPrincipal customPrincipal = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(name), null);
            CustomPrincipal newUser = new CustomPrincipal(coreDB, customPrincipal, null);
            newUser.FirstName = name;
            newUser.LastName = "";
            //newUser.isAdmin = System.Web.Security.Roles.IsUserInRole(name, "Администратор");
            FormsAuthentication.SetAuthCookie(name, true);
            this.Context.Session["user"] = HttpContext.Current.User = newUser;
        }

        void Session_Start(object sender, EventArgs e)
        {
            if (modeAuth == AuthenticationMode.Windows)
            {
                GetCustomPrincipalWindow(User.Identity.Name);
            }
            else if (modeAuth == AuthenticationMode.Forms)
            {
                //GetCustomPrincipalForms("suleevo_temp_user");
            }

        }
    }
}