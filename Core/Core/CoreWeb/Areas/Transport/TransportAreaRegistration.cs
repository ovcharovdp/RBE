using System.Web.Mvc;

namespace CoreWeb.Areas.Transport
{
    public class RBEAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Transport"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Transport_default",
                "Transport/{controller}/{action}/{id}",
                new { action = "Show", id = UrlParameter.Optional }
            );
        }
    }
}