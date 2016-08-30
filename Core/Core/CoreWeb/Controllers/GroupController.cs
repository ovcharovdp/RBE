using BaseEntities;
using CoreAPI.Operations;
using CoreAPI.Types;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.BaseEditModels;
using CoreWeb.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления группами
    /// </summary>
    public class GroupController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public GroupController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "422A100B-EC6E-441E-BC82-464DFBA39557"; } }
        /// <inheritdoc />
        public override ActionResult ShowPartial(long groupID)
        {
            ViewData["groupID"] = groupID;
            return PartialView();
        }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            var m = new EditGroupModel(_db, id, this.ParamGroupID);
            m.Init();
            return m as IEditObjectModel;
        }
        /// <inheritdoc />
        protected override INewObjectModel GetNewObjectModel(long parentID)
        {
            return new NewGroupModel(_db, parentID, ParamGroupID) as INewObjectModel;
        }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long parentID)
        {
            ObjGroup group = new ObjGroup();
            InitObject(group, data);
            ObjGroupOperations.New(CoreDB, parentID, group);
            return new { ID = group.ID, Name = group.Name, HasCld = false, f = 0, spriteCssClass = "folder", Discription = group.Description };
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            ObjGroupOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<CoreAPI.Types.ObjParam.ClientValue> data, long id)
        {
            ObjGroup group = CoreDB.ObjGroups.Find(id);
            if (group == null)
                throw new Exception(string.Format("Не найдена редактируемая группа (ID={0}).", id));
            InitObject(group, data);
            ObjGroupOperations.Check(group);
            CoreDB.SaveChanges();
            return group;
        }
    }
}
