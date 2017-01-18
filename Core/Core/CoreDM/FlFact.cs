


namespace CoreDM
{

using System;
    using System.Collections.Generic;
    using BaseEntities;
    

public partial class FlFact: BaseEntity
{

    public string RegNum { get; set; }

    public string TankFarmCode { get; set; }

    public System.DateTime FactDate { get; set; }

    public decimal Volume { get; set; }

    public int Weight { get; set; }

    public decimal Density { get; set; }

    public int WaybillNum { get; set; }

    public byte ProductCode { get; set; }

    public byte TankNum { get; set; }



    public virtual SysDictionary State { get; set; }

    public virtual FlStation Station { get; set; }

}

}
