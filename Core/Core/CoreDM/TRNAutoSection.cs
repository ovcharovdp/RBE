
namespace CoreDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRNAutoSection
    {
        public long AutoID { get; set; }
        public byte Order { get; set; }
        public decimal Volume { get; set; }
    
        public virtual TRNAuto Auto { get; set; }
    }
}
