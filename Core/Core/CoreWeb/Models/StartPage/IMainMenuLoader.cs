using System.Collections.Generic;
namespace CoreWeb.Models.StartPage
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса основных страниц
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Интерфейс прогрузки главного меню
    /// </summary>
    public interface IMainMenuLoader
    {
        /// <summary>
        /// Список элементов меню
        /// </summary>
        /// <returns>Список пунктов меню</returns>
        List<MainMenuItem> LoadItems();
    }
}
