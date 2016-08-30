using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования параметров
    /// </summary>
    public class EditParamModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор параметра</param>
        /// <param name="fieldGroupID">Идентификатор группы параметров</param>
        public EditParamModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.ObjParams.Find(id);
        }
    }
}