using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using Microsoft.Data.Edm;
using CoreDM;
using BaseEntities;

namespace CoreWeb
{
    /// <summary>
    /// Класс конфигурации WebAPI
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Регистрация маршрутов WebAPI
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapODataServiceRoute("ODataRoute", "odata", GetModel());

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.AddODataQueryFilter();
        }
        private static IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            RegisterUserEntity(builder);
            RegisterStateEntity(builder);
            // для оперативного учета
            builder.EntitySet<SysDictionary>("SysDictionaries");
            builder.EntitySet<OrgDepartment>("OrgDepartments");
            builder.EntitySet<TRNDriver>("TRNDrivers");
            builder.EntitySet<FlStation>("FlStations");
            builder.EntitySet<FlOrder>("FlOrders");
            builder.EntitySet<TRNAuto>("TRNAutos");
            builder.EntitySet<TRNAutoSection>("TRNAutoSections");
            builder.EntitySet<FlOrderItem>("FlOrderItems");

            return builder.GetEdmModel();
        }
        /// <summary>
        /// Регистрация сущностей OData для работы с пользователями
        /// </summary>
        /// <param name="builder">Построитель модели</param>
        public static void RegisterUserEntity(ODataConventionModelBuilder builder)
        {
            builder.EntitySet<SysRole>("SysRoles");
            var users = builder.EntitySet<SysUser>("SysUsers");
            users.EntityType.Ignore(p => p.Password);
            users.EntityType.Action("RunRule").ReturnsFromEntitySet<SysUser>("SysUsers");

            builder.EntitySet<SysUserRole>("SysUserRoles");
        }
         /// <summary>
        /// Регистрация сущностей OData для работы с состояниями и правилами
        /// </summary>
        /// <param name="builder">Построитель модели</param>
        public static void RegisterStateEntity(ODataConventionModelBuilder builder)
        {
            builder.EntitySet<ObjCatalog>("ObjCatalogs");
            builder.EntitySet<ObjCatalogState>("ObjStates");
            builder.EntitySet<ObjCatalogRule>("ObjRules");
        }
    }
}