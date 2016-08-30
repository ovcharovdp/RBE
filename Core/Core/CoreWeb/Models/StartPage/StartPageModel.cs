using System.Collections.Generic;
using System.Web.Configuration;

namespace CoreWeb.Models.StartPage
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса основной страницы
    /// </summary>
    public class StartPageModel
    {
        /// <summary>
        /// Заголовок начальной страницы приложения
        /// </summary>
        public string Title { get { return WebConfigurationManager.AppSettings.Get("AppTitle"); } }
        /// <summary>
        /// Интерфейс прогрузки главного меню
        /// </summary>
        public IMainMenuLoader _menuLoader;
        private AuthenticationMode _modeAuth;
        /// <summary>
        /// Метод аутентификации
        /// </summary>
        public AuthenticationMode modeAuth
        {
            get
            {
                return _modeAuth;
            }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="menuLoader">Интерфейс прогрузки главного меню</param>
        public StartPageModel(IMainMenuLoader menuLoader)
        {
            _menuLoader = menuLoader;
            _modeAuth = (WebConfigurationManager.OpenWebConfiguration("~").GetSection("system.web/authentication") as AuthenticationSection).Mode;
        }
        private List<MainMenuItem> _menuItems;
        /// <summary>
        /// Список элементов меню
        /// </summary>
        public List<MainMenuItem> MenuItems
        {
            get
            {
                if (_menuItems == null)
                {
                    _menuItems = _menuLoader.LoadItems();
                    _menuItems.Add(new MainMenuItem() { Url = "~/Help/index.html", ImageName = "help", Description = "Помощь", Type = "link", Name = "Помощь" });
                }
                return _menuItems;
            }
        }
    }
}
