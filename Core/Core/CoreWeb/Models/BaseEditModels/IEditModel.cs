using CoreWeb.Models.Params;
using System.Collections.Generic;

namespace CoreWeb.Models.BaseEditModels
{
    /// <summary>
    /// Интерфейс, описывающий модель для окна создания объекта
    /// </summary>
    public interface INewObjectModel
    {
        /// <summary>
        /// Действие при нажатии на кнопку "Сохранить" (код JavaScript)
        /// </summary>
        string SaveEvent { get; }
        /// <summary>
        /// Действие при нажатии на кнопку "Отмена" (код JavaScript)
        /// </summary>
        string CancelEvent { get; }
        /// <summary>
        /// Список параметров создаваемого объекта, выводимых в окно создания/редактирования
        /// </summary>
        IEnumerable<ParamSettings> Params { get; }
    }
    /// <summary>
    /// Интерфейс, описывающий модель для окна редактирования объекта
    /// </summary>
    public interface IEditObjectModel : INewObjectModel
    {
        /// <summary>
        /// Идентификатор редактируемого объекта
        /// </summary>
        long ID { get; }
    }
}
