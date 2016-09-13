
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    public partial class FlOrder: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public FlOrder()
        {
            this.Items = new HashSet<FlOrderItem>();
        }
    
        public System.DateTime DocDate { get; set; }
    
        public virtual TRNAuto Auto { get; set; }
    	[JsonIgnore]
        public virtual ICollection<FlOrderItem> Items { get; set; }
        public virtual OrgDepartment TankFarm { get; set; }
    }
}
