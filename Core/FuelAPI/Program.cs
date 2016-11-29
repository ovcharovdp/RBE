using FuelAPI.Config;
using FuelAPI.TTN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            FuelConfig fuelConfig =
       (FuelConfig)System.Configuration.ConfigurationManager.GetSection(
       "fuelConfigGroup/fuelConfig");

            new Logistica().UploadWaybills();

            var h = new TTN.Handler(fuelConfig); h.GetFileList(); h.DownloadFiles();
            var h1 = new TTN.EMailHandler(fuelConfig); h1.DownloadFiles();
             var s = new FtpSender(fuelConfig); s.Send();
          //  Console.WriteLine("Загружено");
         //    Console.ReadKey();
        }
    }
}
