using System;
using TankBalance.Config;
using TankBalance.Loader;

namespace TankBalance
{

    class Program
    {
        static void Main(string[] args)
        {
            BalanceConfig config = (BalanceConfig)System.Configuration.ConfigurationManager.GetSection("balanceConfigGroup/balanceConfig");
            System.IO.StreamWriter file = new System.IO.StreamWriter(".\\Logs\\" + DateTime.Today.ToString("yyyy_MM_dd") + ".log", true);
            file.WriteLine("*** " + DateTime.Now.ToShortTimeString() + " ***");
            try
            {
                var l = new BalanceLoader(config, file); l.Load();
            }
            catch (Exception e)
            {
                file.WriteLine(e.Message);
            }
            file.Close();
        }
    }
}
