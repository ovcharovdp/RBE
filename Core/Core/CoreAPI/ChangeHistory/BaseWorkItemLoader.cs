using System;
using System.Collections.Generic;

namespace CoreAPI.ChangeHistory
{
    /// <summary>
    /// Заглушка реализации для получения информации по изменениям в системе
    /// </summary>
    public class BaseWorkItemLoader : IWorkItemLoader
    {
        private List<WorkItem> _items;
        /// <summary>
        /// Возвращает список рабочих элементов проекта
        /// </summary>
        public List<WorkItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<WorkItem>();
                    _items.Add(new WorkItem() { IterationFinishDate = DateTime.Now.AddDays(-1), IterationName = "1.0.0", Title = "Функционал1", Type = "sds" });
                    _items.Add(new WorkItem() { IterationFinishDate = DateTime.Now.AddDays(-1), IterationName = "1.0.0", Title = "Функционал2", Type = "sds" });
                    _items.Add(new WorkItem() { IterationFinishDate = DateTime.Now.AddDays(-1), IterationName = "1.0.0", Title = "Ошибка", Type = "Ошибка" });
                    _items.Add(new WorkItem() { IterationFinishDate = DateTime.Now.AddDays(1), IterationName = "1.1.0", Title = "Функционал3", Type = "sds" });
                }
                return _items;
            }
        }
        /// <summary>
        /// Выполняет загрузку рабочих элементов из источника данных
        /// </summary>
        /// <param name="projectID">Идентификатор командного проекта</param>
        public void Load(string projectID) { }
    }
}
