using System.Linq;
using System.Collections.Generic;
using CoreAPI.Types.ObjParam;
using CoreAPI.Operations;
using System;
using CoreWeb.Controllers.Base;
using CoreWeb.Models.EditModels;
using CoreWeb.Models.BaseEditModels;
using BaseEntities;
using CoreAPI.Types;

namespace CoreWeb.Controllers
{
    /// <summary>
    /// Контроллер управления словарями
    /// </summary>
    public class DictionaryController : BaseGroupEntityController
    {
        /// <inheritdoc />
        public DictionaryController(ICoreDBContext db) : base(db) { }
        /// <inheritdoc />
        protected override string CatalogGID { get { return "EB84B9E5-043A-4621-BCDD-6058AA512B00"; } }
        /// <inheritdoc />
        protected override IEditObjectModel GetEditObjectModel(long id)
        {
            return new DictionaryModel(_db, id, ParamGroupID) as IEditObjectModel;
        }
        /// <inheritdoc />
        protected override object IntNew(IEnumerable<ClientValue> data, long parentID)
        {
            SysDictionary element = new SysDictionary();
            InitObject(element, data);
            SysDictionaryOperations.New(CoreDB, parentID, element);
            return element;
        }
        /// <inheritdoc />
        protected override void IntDel(long id)
        {
            SysDictionaryOperations.Del(CoreDB, id);
        }
        /// <inheritdoc />
        protected override object IntEdit(IEnumerable<ClientValue> data, long id)
        {
            SysDictionary element = CoreDB.SysDictionaries.Find(id);
            if (element == null)
                throw new Exception(string.Format("Не найден редактируемый элемент словаря (ID={0}).", id));
            if (InitObject(element, data))
            {
                SysDictionaryOperations.Check(element);
                CoreDB.SaveChanges();
            }
            return element;
        }
        /// <inheritdoc />
        protected override IQueryable GetObjectList(long groupID)
        {
            return from g in CoreDB.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == groupID)
                   join d in CoreDB.SysDictionaries.AsNoTracking() on g.ObjectID equals d.ID
                   select d;
        }
    }
}
