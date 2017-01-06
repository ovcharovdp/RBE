
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;

    public partial class FlOrderItem: BaseEntity
    {

	    /// <summary>
	    /// Конструктор
	    /// </summary>
        public FlOrderItem()
        {
            this.TankNum = 0;
            this.VolumeFact = 0m;
        }

        public byte SectionNum { get; set; }
        public byte TankNum { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumeFact { get; set; }
        public Nullable<int> WaybillNum { get; set; }
        public Nullable<System.DateTime> WaybillDate { get; set; }

        public Nullable<decimal> Density { get; set; }
        public Nullable<decimal> Temperature { get; set; }
        public string QPassportNum { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public Nullable<System.DateTime> QPassportDate { get; set; }

        public Nullable<int> Weight { get; set; }

	    /// <summary>
	    /// Плотность продукта при температуре приведения
	    /// </summary>

        public Nullable<decimal> QDensity { get; set; }

        public virtual FlOrder Order { get; set; }
        public virtual OrgDepartment Customer { get; set; }
        public virtual SysDictionary Product { get; set; }
        public virtual SysDictionary State { get; set; }
        public virtual FlStation Station { get; set; }
    }
}
