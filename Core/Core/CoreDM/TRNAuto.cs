
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using BaseEntities;
    
    public partial class TRNAuto: BaseEntity
    {
    	/// <summary>
    	/// Конструктор
    	/// </summary>
        public TRNAuto()
        {
            this.Sections = new HashSet<TRNAutoSection>();
        }
    
        public string RegNum { get; set; }
        public string RegNumExt { get; set; }
        public System.DateTime NextCertDate { get; set; }
        public Nullable<int> Volume { get; set; }
    
        public virtual OrgDepartment Organization { get; set; }
        public virtual SysDictionary Model { get; set; }
    	[JsonIgnore]
        public virtual ICollection<TRNAutoSection> Sections { get; set; }
    }
}
