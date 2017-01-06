using System;

namespace TankBalance.Loader
{
    public class TankData
    {
        public byte Num { get; set; }
        public byte ProductCode { get; set; }
        public int Volume { get; set; }
        public int StartVolume { get; set; }
        public DateTime BalanceDate { get; set; }
        public int InputVolume { get; set; }
    }
}
