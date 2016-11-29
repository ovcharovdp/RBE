
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using BaseEntities;

    public partial class FlStation: BaseEntity
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public short Number { get; set; }
	    /// <summary>
	    /// Код АСУТП
	    /// </summary>
        public Nullable<int> Code { get; set; }
        public string InfoOilCode { get; set; }

        public virtual OrgDepartment Organization { get; set; }
        public virtual SysDictionary Type { get; set; }
    }
}
