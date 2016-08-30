using CoreAPI.Types;
using CoreWeb.Models.BaseEditModels;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования словарей
    /// </summary>
    public class DictionaryModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор словаря</param>
        /// <param name="fieldGroupID">Идентификатор группы полей</param>
        public DictionaryModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.SysDictionaries.Find(id);
        }
    }
}