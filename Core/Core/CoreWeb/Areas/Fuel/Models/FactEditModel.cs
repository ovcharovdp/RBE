using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Areas.Fuel.Models
{
    public class FactEditModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор роли</param>
        /// <param name="fieldGroupID">Идентификатор группы ролей</param>
        public FactEditModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.FlFacts.Find(id);
        }
    }
}