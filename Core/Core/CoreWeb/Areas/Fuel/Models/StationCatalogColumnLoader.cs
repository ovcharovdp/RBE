using BaseEntities;
using CoreAPI.Const;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreWeb.Areas.Fuel.Models
{
    public class StationCatalogColumnLoader : ColumnLoaderFromFields
    {
        public StationCatalogColumnLoader(CoreEntities db, ICollection<ObjCatalogField> fields)
            : base(db, fields)
        { }
        protected override void LoadColumns()
        {
            base.LoadColumns();
            var c = Columns.FirstOrDefault(p => p.Name.Equals("Organization.ID"));
            if (c != null)
            {
                var q = _db.OrgDepartments.AsNoTracking().Where(p => p.Parent.ID == 102);
                c.Values = q.Select(p => new BaseObject() { ID = p.ID, Name = p.FullName }).ToList();
            }
            c = Columns.FirstOrDefault(p => p.Name.Equals("Type.ID"));
            if (c != null)
            {
                var gID = ConstManager.Get("26C95960-6671-4242-82FA-88CCCF716486", new GroupIDLoader(_db));
                var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == gID)
                        join d in _db.SysDictionaries.AsNoTracking() on g.ObjectID equals d.ID
                        select new BaseObject() { ID = d.ID, Name = d.Name };
                c.Values = q.ToList();
            }
        }
    }
}