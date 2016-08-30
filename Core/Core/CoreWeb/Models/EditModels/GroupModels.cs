using CoreWeb.Models.BaseEditModels;
using CoreAPI.Types;

namespace CoreWeb.Models.EditModels
{
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса создания групп
    /// </summary>
    public class NewGroupModel : BaseNewGroupedElementModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="parentID">Идентификатор родителя</param>
        /// <param name="fieldGroupID">Идентификатор группы, содержащей настройки полей редактирования</param>
        public NewGroupModel(ICoreDBContext db, long parentID, long fieldGroupID)
            : base(db, parentID, fieldGroupID)
        { }
        /// <inheritdoc />
        public override string SaveEvent
        {
            get { return string.Format("newGroup(\"{0}\")", this._parentID); }
        }
    }
    /// <summary>
    /// Класс реализации модели пользовательского интерфейса редактирования групп
    /// </summary>
    public class EditGroupModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор группы</param>
        /// <param name="fieldGroupID">Идентификатор группы, в которой храняться параметры для описания правил заполнения полей класса</param>
        public EditGroupModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.ObjGroups.Find(id);
        }
        /// <inheritdoc />
        public override string SaveEvent
        {
            get { return string.Format("editGroup({0})", this.ID); }
        }
    }
}