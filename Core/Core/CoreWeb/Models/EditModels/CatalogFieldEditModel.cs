using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Реализует модель редактирования поля журнала
    /// </summary>
    public class CatalogFieldEditModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор поля журнала</param>
        /// <param name="fieldGroupID">Идентификатор группы полей журнала</param>
        public CatalogFieldEditModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.ObjCatalogFields.Find(id);
        }
    }
}