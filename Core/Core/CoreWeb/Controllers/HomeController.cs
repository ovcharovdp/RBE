using CoreAPI.ChangeHistory;
using CoreWeb.Attributes.MVC;
using CoreWeb.Models.StartPage;
using System.Web.Mvc;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления первоначальной страницей
    /// </summary>
    [ExtAuthorize]
    public class HomeController : Controller
    {
        private IMainMenuLoader _menuLoader;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="menuLoader">Меню</param>
        public HomeController(IMainMenuLoader menuLoader)
        {
            _menuLoader = menuLoader;
        }
        /// <summary>
        /// Первоначальная страница
        /// </summary>
        /// <returns>Основная страница приложения</returns>
        public ActionResult Index()
        {
            StartPageModel m = new StartPageModel(_menuLoader);
            return View(m);
        }
        /// <summary>
        /// История изменений
        /// </summary>
        /// <param name="projectID">Идентификатор командного проекта</param>
        /// <returns>Представление со сведениями об истории изменений</returns>
        public ActionResult ChangeHistory(string projectID)
        {
            IWorkItemLoader l = DependencyResolver.Current.GetService<IWorkItemLoader>();
            if (l == null)
            {
                return PartialView("EmptyChangeHistory");
            }
            else
            {
                l.Load(projectID);
                return PartialView(l);
            }
        }
    }
}
