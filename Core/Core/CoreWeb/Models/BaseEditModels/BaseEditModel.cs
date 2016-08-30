using CoreAPI.Types;
using CoreWeb.Models.Params;

namespace CoreWeb.Models.BaseEditModels
{
    /// <summary>
    /// Базовый класс для реализации моделей пользовательского интерфейса редактирования объекта
    /// </summary>
    public abstract class BaseEditModel : ParamListModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="fieldGroupID">Идентификатор группы, в которой храняться параметры для описания правил заполнения полей класса</param>
        public BaseEditModel(ICoreDBContext db, long fieldGroupID) : base(db, 0, fieldGroupID) { }
    }
}