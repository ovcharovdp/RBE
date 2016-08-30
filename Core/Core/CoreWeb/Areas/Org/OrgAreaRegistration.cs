using System.Web.Mvc;

namespace CoreWeb.Areas.Org
{
    /// <summary>
    /// Пространство имен для описания области "Организации".
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <inheritdoc />
    public class OrgAreaRegistration : AreaRegistration
    {
        /// <inheritdoc />
        public override string AreaName { get { return "Org"; } }
        /// <inheritdoc />
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Org_default",
                "Org/{controller}/{action}/{id}",
                new { action = "Show", id = UrlParameter.Optional }
            );
        }
    }
}
