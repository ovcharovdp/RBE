using CoreWeb.Authorization;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CoreWeb.Attributes.OData
{
    /// <summary>
    /// Реализует расширенное управление доступом
    /// </summary>
    public class ExtAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Признак доступа только для администратора системы
        /// </summary>
        public bool AdminOnly { get; set; }
        /// <inheritdoc />
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var p = actionContext.RequestContext.Principal as CustomPrincipal;
            return !p.IsBlocked && ((AdminOnly && p.isAdmin) || !AdminOnly);
        }
        /// <inheritdoc />
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                var p = actionContext.RequestContext.Principal as CustomPrincipal;
                if (AdminOnly && !p.isAdmin && actionContext.Request.Headers.Contains("X-Requested-With"))
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    {
                        Content = new System.Net.Http.StringContent("Информация доступна только администратору системы")
                    };
                }
                else
                {
                    base.HandleUnauthorizedRequest(actionContext);
                }
            }
            else
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
        }
    }
}