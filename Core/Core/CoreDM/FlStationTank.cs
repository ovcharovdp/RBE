


namespace CoreDM
{

using System;
    using System.Collections.Generic;
    using BaseEntities;
    

public partial class FlStationTank: BaseEntity
{

	/// <summary>
	/// Конструктор
	/// </summary>
    public FlStationTank()
    {

        this.Volume = 0;

        this.DeadVolume = 0;

        this.Balance = 0;

        this.DaySell = 0;

    }


    public byte Num { get; set; }

    public int Volume { get; set; }

    public int DeadVolume { get; set; }

    public int Balance { get; set; }

    public System.DateTime BalanceDate { get; set; }

    public int DaySell { get; set; }

    public byte SellDays { get; set; }

	/// <summary>
	/// Прогнозная дата высыхания
	/// </summary>

    public System.DateTime DeadDate { get; set; }

    public byte ProductCode { get; set; }


    public virtual FlStation Station { get; set; }

    public virtual SysDictionary State { get; set; }

    public virtual SysDictionary Product { get; set; }

}

}
