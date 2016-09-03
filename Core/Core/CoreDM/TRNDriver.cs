
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;
    
    public partial class TRNDriver: BaseEntity
    {
        public string Name { get; set; }
        public string DocNum { get; set; }
        public string DocPlace { get; set; }
        public Nullable<System.DateTime> DocDate { get; set; }
        public string RegAddress { get; set; }
    
        public virtual OrgDepartment Organization { get; set; }
    }
}
