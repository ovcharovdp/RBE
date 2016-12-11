using BaseEntities;
using CoreAPI.Const;
using CoreAPI.Types;
using CoreDM;
using CoreWeb.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace CoreWeb.Areas.Fuel.Models
{
    public class OrderCatalogColumnLoader : ColumnLoaderFromFields
    {
        public OrderCatalogColumnLoader(CoreEntities db, ICollection<ObjCatalogField> fields)
            : base(db, fields)
        { }
        protected override void LoadColumns()
        {
            base.LoadColumns();
            IConstLoader gl = new GroupIDLoader(_db);
            var c = Columns.FirstOrDefault(p => p.Name.Equals("TankFarm.ID"));
            if (c != null)
            {
                var gID = ConstManager.Get("8E6BB8D5-52E3-48E3-AD7E-61071CCAF7FC", gl);
                var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == gID)
                        join d in _db.OrgDepartments.AsNoTracking() on g.ObjectID equals d.ID
                        select new BaseObject() { ID = d.ID, Name = d.Name };
                c.Values = q.Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToList();
            }
            c = Columns.FirstOrDefault(p => p.Name.Equals("Auto.Organization.ID"));
            if (c != null)
            {
                // var gID = ConstManager.Get("A160B3B8-52D3-43F4-8DB0-ACF01A2F6344", gl);
                //var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == gID)
                //        join d in _db.OrgDepartments.AsNoTracking() on g.ObjectID equals d.ID
                //        select new BaseObject() { ID = d.ID, Name = d.Name };
                var q = _db.OrgDepartments.AsNoTracking().Where(p => p.Type.ID == 205).Select(p => new BaseObject() { ID = p.ID, Name = p.Name });
                c.Values = q.ToList();
            }
            c = Columns.FirstOrDefault(p => p.Name.Equals("State.ID"));
            if (c != null)
            {
                var gID = ConstManager.Get("A991679A-9FD5-49CC-B69D-83E0DF8610CA", gl);
                var q = from g in _db.ObjGroupObjects.AsNoTracking().Where(p => p.GroupID == gID)
                        join d in _db.SysDictionaries.AsNoTracking() on g.ObjectID equals d.ID
                        select d;
                c.Values = q.Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToList();
            }
            c = Columns.FirstOrDefault(p => p.Name.Equals("Customer.ID"));
            if (c != null)
            {
                var q = _db.OrgDepartments.AsNoTracking().Where(p => p.Type.ID == 104).Select(p => new BaseObject() { ID = p.ID, Name = p.FullName });
                c.Values = q.Select(p => new BaseObject() { ID = p.ID, Name = p.Name }).ToList();
            }
        }
    }
}