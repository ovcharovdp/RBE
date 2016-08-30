using System;
using System.Web.Mvc;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Управление  ошибками приложения
    /// </summary>
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Страница ошибок
        /// </summary>
        /// <param name="id">Код ошибки</param>
        /// <returns>Представление</returns>
        public ActionResult Oops(int id)
        {
            Response.StatusCode = id;
            switch (id)
            {
                case 400:
                    Response.StatusDescription = "Неверный запрос";
                    break;
                case 401:
                    Response.StatusDescription = "Неавторизованный запрос";
                    break;
                case 403:
                    Response.StatusDescription = "Доступ к ресурсу запрещен";
                    break;
                case 404:
                    Response.StatusDescription = "Запрашиваемый ресурс не найден";
                    break;
                case 500:
                    Response.StatusDescription = "Внутренняя ошибка сервера";
                    break;
                default:
                    Response.StatusDescription = "Неизвестная ошибка";
                    break;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["msg"]))
                return View("Error", new HandleErrorInfo(new Exception(Request.QueryString["msg"]), "Custom", "Custom"));
            else
                return View("Error");

        }
        /// <summary>
        /// Возвращает представление о блокировке пользователя
        /// </summary>
        /// <returns>Представление</returns>
        public ActionResult UserLock()
        {
            Response.StatusCode = 403;
            Response.StatusDescription = "Пользователь заблокирован администратором системы.";
            return View("Error");
        }
    }
}