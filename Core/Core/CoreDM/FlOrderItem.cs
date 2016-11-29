
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;
    
    public partial class FlOrderItem: BaseEntity
    {
        public decimal SectionNum { get; set; }
        public Nullable<decimal> TankNum { get; set; }
        public int Volume { get; set; }
        public Nullable<int> VolumeFact { get; set; }
        public Nullable<int> WaybillNum { get; set; }
        public Nullable<System.DateTime> WaybillDate { get; set; }

        public Nullable<decimal> Density { get; set; }
        public Nullable<decimal> Temperature { get; set; }
        public string QPassportNum { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public Nullable<System.DateTime> QPassportDate { get; set; }

        public virtual FlOrder Order { get; set; }
        public virtual OrgDepartment Customer { get; set; }
        public virtual SysDictionary Product { get; set; }
        public virtual SysDictionary State { get; set; }
        public virtual FlStation Station { get; set; }
    }
}
