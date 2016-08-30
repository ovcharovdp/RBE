using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Пространство имен для реализации моделей пользовательского интерфейса редактирования сущностей
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования полей журналов
    /// </summary>
    public class CatalogEditModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор журнала</param>
        /// <param name="fieldGroupID">Идентификатор группы полей журнала</param>
        public CatalogEditModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.ObjCataloges.Find(id);
        }
    }
}