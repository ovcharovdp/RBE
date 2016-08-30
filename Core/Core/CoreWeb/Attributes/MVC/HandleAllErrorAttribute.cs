using CoreAPI.Types;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace CoreWeb.Attributes.MVC
{
    /// <summary>
    /// Собственный атрибут, используемый для обработки исключения, вызываемого методом действия.
    /// Он обрабатывает только исключения, устанавливающие код ответа сервера в 500.
    /// </summary>
    public class HandleAllErrorAttribute : HandleErrorAttribute
    {
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        /// <summary>
        /// Событие вызова ошибки
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            //Если запрос - AJAX, то возвращаем JSON, иначе view-представление.
            if (IsAjax(filterContext))
            {
                if (filterContext.HttpContext.Request.QueryString["callback"] == null)
                    filterContext.Result = new JsonResult()
                    {
                        Data = new { msg = ErrorManager.GetFullMessage(filterContext.Exception) },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                else
                    filterContext.Result = new JavaScriptResult()
                    {
                        Script = string.Format("{0}({1})", filterContext.HttpContext.Request.Params["callback"], JsonConvert.SerializeObject(new { msg = ErrorManager.GetFullMessage(filterContext.Exception) }))
                    };

                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                base.OnException(filterContext);
            }

            //Ниже можно описать логирование ошибок.
        }
    }
}