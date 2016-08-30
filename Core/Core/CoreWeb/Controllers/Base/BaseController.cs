using CoreWeb.Attributes.MVC;
using System.Web.Mvc;

namespace CoreWeb.Controllers.Base
{
    /// <summary>
    /// Пространство имен для реализации базовых контроллеров в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Базовый контроллер, от которого должны наследоваться все контроллеры, реализуемые в рамках платформы.
    /// Предоставляет доступ к действиям только в случае авторизации пользователя (содержит атрибут [Authorize] на уровне контроллера).
    /// </summary>
    [ExtAuthorize]
    public abstract class BaseController : Controller { }
}