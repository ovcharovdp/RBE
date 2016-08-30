using System.Collections.Generic;

namespace CoreAPI.ChangeHistory
{
    /// <summary>
    /// Пространство имен для реализации базовых типов получения данных по изменениям программного продукта
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <summary>
    /// Интерфейс загрузчика информации по изменениям в системе
    /// </summary>
    public interface IWorkItemLoader
    {
        /// <summary>
        /// Список рабочих элементов
        /// </summary>
        List<WorkItem> Items { get; }
        /// <summary>
        /// Загружает рабочие элементы из источника данных
        /// </summary>
        /// <param name="projectID">Идентификатор командного проекта</param>
        void Load(string projectID);
    }
}
