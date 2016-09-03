using BaseEntities;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Areas.Transport.Models
{
    public class DriverCatalogColumnLoader : ColumnLoaderFromFields
    {
        public DriverCatalogColumnLoader(CoreEntities db, ICollection<ObjCatalogField> fields)
            : base(db, fields)
        { }
        protected override void LoadColumns()
        {
            base.LoadColumns();
            var c=Columns.FirstOrDefault(p => p.Name.Equals("Organization.ID"));
            if(c != null)
            {
                var q = from og in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == 87)
                           join d in _db.OrgDepartments.AsNoTracking() on og.ObjectID equals d.ID
                           select new BaseObject() { ID = d.ID, Name = d.Name };
                c.Values = q.ToList();
            }
        }
    }
}