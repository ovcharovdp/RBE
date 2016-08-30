using System;

namespace CoreAPI.ChangeHistory
{
    /// <summary>
    /// Рабочий элемент командного проекта
    /// </summary>
    public class WorkItem
    {
        /// <summary>
        /// Название итерации
        /// </summary>
        public string IterationName { get; set; }
        /// <summary>
        /// Тип рабочего элемента
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Заголовок рабочего элемента
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Состояние
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Дата завершения транзакции
        /// </summary>
        public DateTime IterationFinishDate { get; set; }
    }
}
