using CoreAPI;
using CoreAPI.Const;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreAPI.Types.ObjParam;
using CoreWeb.Models.BaseEditModels;
using System;
using System.Linq;

namespace CoreWeb.Areas.Org.Models
{
    /// <summary>
    /// Модель редактирования организации
    /// </summary>
    public class NewDepartmentModel : BaseNewGroupedElementModel
    {
        /// <inheritdoc />
        public NewDepartmentModel(ICoreDBContext db, long parentID, long fieldGroupID) : base(db, parentID, fieldGroupID) { }
        /// <inheritdoc />
        /// <exception cref="MemberAccessException">При отсутствии поля сущности, к которому применяется логика обработки</exception>
        protected override void PostInitFields()
        {
            base.PostInitFields();
            var _p = _params.FirstOrDefault(p => p.Code.Equals("TypeID"));
            if (_p == null)
                throw new MemberAccessException("Не определено поле \"TypeID\"");
            var _v = SysDictionaryOperations.GetItems(CoreDB,
                ConstManager.Get(CoreConstant.ORG_TYPE_GROUP_GID, this._db)).Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToArray();
            _p.Items.AddRange(_v.Select(p => new ParamItemValue() { Label = p.Name, Name = p.ID.ToString() }));
        }
    }
}