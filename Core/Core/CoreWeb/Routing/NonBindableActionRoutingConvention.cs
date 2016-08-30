using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;
using Microsoft.Data.Edm;

namespace CoreWeb.Routing
{
    /// <summary>
    /// Пространство имен для описания правил маршрутизации запрососв
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <summary>
    /// Implements a routing convention for non-bindable actions.
    /// The convention maps "MyAction" to Controller:MyAction() method, where the name of the controller 
    /// is specified in the constructor.
    /// </summary>
    public class NonBindableActionRoutingConvention : IODataRoutingConvention
    {
        private string _controllerName;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controllerName">Наименование контроллера</param>
        public NonBindableActionRoutingConvention(string controllerName)
        {
            _controllerName = controllerName;
        }
        /// <summary>
        /// Route all non-bindable actions to a single controller.
        /// </summary>
        /// <param name="odataPath">Путь</param>
        /// <param name="request">Запрос</param>
        /// <returns>Наименование контроллера</returns>
        public string SelectController(ODataPath odataPath, System.Net.Http.HttpRequestMessage request)
        {
            if (odataPath.PathTemplate == "~/action")
            {
                return _controllerName;
            }
            return null;
        }

        /// <summary>
        /// Route the action to a method with the same name as the action.
        /// </summary>
        /// <param name="odataPath">Путь</param>
        /// <param name="controllerContext">Контекст контроллера</param>
        /// <param name="actionMap">Сопоставление действий</param>
        /// <returns>Наименование действия</returns>
        public string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            // OData actions must be invoked with HTTP POST.
            if (controllerContext.Request.Method == HttpMethod.Get)
            {
                if (odataPath.PathTemplate == "~/action")
                {
                    ActionPathSegment actionSegment = odataPath.Segments.Last() as ActionPathSegment;
                    IEdmFunctionImport action = actionSegment.Action;

                    if (!action.IsBindable && actionMap.Contains(action.Name))
                    {
                        return action.Name;
                    }
                }
            }
            return null;
        }
    }
}