using System.Web.Optimization;

namespace CoreWeb
{
    /// <summary>
    /// Класс конфигурации взаимодействий
    /// </summary>
    public class BundleConfig
    {
        //Дополнительные сведения об объединении см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// Регистрация взаимодействий
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.DirectoryFilter.Clear();
            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                        "~/Content/kendo/kendo.common.min.css",
                        "~/Content/kendo/kendo.dataviz.min.css",
                        "~/Content/kendo/*.default.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/common.js",
                        "~/Scripts/param.js",
                        "~/Scripts/fuel.js",
                        "~/Scripts/orgExt.js",
                        "~/Scripts/jquery.multilevelpushmenu.js",
                        "~/Scripts/kendo/jszip.min.js",
                        "~/Scripts/kendo/cultures/kendo.culture.ru-RU.min.js",
                        "~/Scripts/kendo/kendo.ext.js",
                        "~/Scripts/kendo/kendo.ru-RU.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",
                "~/Content/Menu/jquery.multilevelpushmenu_white.css"));
        }
    }
}