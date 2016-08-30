using CoreWeb.Authorization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CoreWeb.Attributes.MVC
{
    /// <summary>
    /// Обработчик проверки авторизации пользователя в системе
    /// </summary>
    public class ExtAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Признак проверки на права администратора
        /// </summary>
        public bool AdminOnly { get; set; }
        /// <inheritdoc />
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var p = httpContext.User as CustomPrincipal;
            return base.AuthorizeCore(httpContext) &&
                !p.IsBlocked && ((AdminOnly && p.isAdmin) || !AdminOnly);
        }
        private bool IsAjax(AuthorizationContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        /// <inheritdoc />
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                base.HandleUnauthorizedRequest(filterContext);
            else
            {
                var user = filterContext.HttpContext.User as CustomPrincipal;
                if (user.IsBlocked)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Error", action = "UserLock" }));
                }
                else
                {
                    if (AdminOnly && !user.isAdmin)
                    {
                        if (IsAjax(filterContext))
                        {
                            filterContext.Result = new ContentResult() { Content = "Операция доступна только администратору системы" };
                            filterContext.HttpContext.Response.StatusCode = 403;
                        }
                        else
                        {
                            filterContext.Result = new RedirectToRouteResult(new
                               RouteValueDictionary(new { controller = "Error", action = "Oops", id = 403, msg = "Операция доступна только администратору системы" }));
                        }
                    }
                    else
                    {
                        base.HandleUnauthorizedRequest(filterContext);
                    }
                }
            }
        }
    }
}