
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;
    
    public partial class FlOrderItem
    {
        public long OrderID { get; set; }
        public byte SectionNum { get; set; }
        public Nullable<byte> TankNum { get; set; }
        public int Volume { get; set; }
        public Nullable<int> WaybillNum { get; set; }
        public Nullable<System.DateTime> WaybillDate { get; set; }
    
        public virtual FlOrder Order { get; set; }
        public virtual SysDictionary Product { get; set; }
        public virtual FlStation Station { get; set; }
    }
}
