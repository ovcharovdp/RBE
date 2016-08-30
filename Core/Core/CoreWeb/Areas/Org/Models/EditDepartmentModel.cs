using CoreAPI;
using CoreAPI.Const;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using System.Linq;

namespace CoreWeb.Areas.Org.Models
{
    /// <summary>
    /// Пространство имен для описания моделей организаций.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Модель редактирования организации
    /// </summary>
    public class EditDepartmentModel : BaseEditExistsModel
    {
        /// <summary>
        /// Конкструктор модели редактирования организации
        /// </summary>
        /// <param name="db">Провайдер доступа к модели данных</param>
        /// <param name="id">Идентификатор организации</param>
        /// <param name="fieldGroupID">Идентификатор группы поля</param>
        public EditDepartmentModel(ICoreDBContext db, long id, long fieldGroupID)
            : base(db, id, fieldGroupID)
        {
            _element = CoreDB.OrgDepartments.Find(id);
        }
        /// <summary>
        /// Инициализация полей интерфейса
        /// </summary>
        protected override void PostInitFields()
        {
            base.PostInitFields();
            var _p = _params.FirstOrDefault(p => p.Code.Equals("TypeID"));
            var _v = SysDictionaryOperations.GetItems(CoreDB,
                ConstManager.Get(CoreConstant.ORG_TYPE_GROUP_GID, this._db)).Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToArray();
            _p.Items.AddRange(_v.Select(p => new ParamItemValue() { Label = p.Name, Name = p.ID.ToString() }));
        }
    }
}