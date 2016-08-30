using System.Collections.Generic;

namespace CoreWeb.Models.StartPage
{
    /// <summary>
    /// Элемент главного меню
    /// </summary>
    public class MainMenuItem
    {
        /// <summary>
        /// Идентификатор элемента меню
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Наименование иконки
        /// </summary>
        public string ImageName { get; set; }
        /// <summary>
        /// Ссылка перехода
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Параметры
        /// </summary>
        public string Params { get; set; }
        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Порядок
        /// </summary>
        public short Order { get; set; }
        /// <summary>
        /// Видимость
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// Список дочерних элементов
        /// </summary>
        public List<MainMenuItem> Children { get; set; }
    }
}