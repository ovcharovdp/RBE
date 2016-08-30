using CoreAPI.Types;
using CoreDM;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования ролей
    /// </summary>
    public class UserModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор роли</param>
        /// <param name="fieldGroupID">Идентификатор группы ролей</param>
        public UserModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.SysUsers.Find(id);
        }
    }
}