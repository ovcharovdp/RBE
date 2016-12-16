using FuelAPI.Config;
using FuelAPI.TTN;
using System;

namespace FuelAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            FuelConfig fuelConfig = (FuelConfig)System.Configuration.ConfigurationManager.GetSection("fuelConfigGroup/fuelConfig");

            //  new Logistica.LogisticaChuv().UploadWaybills();
            // загрузка плана
            new Logistica.Logistica().UploadWaybills();
            // загрузка факта с FTP
            var h = new TTN.Handler(fuelConfig); h.Handle();
            // загрузка факта с почты
            var h1 = new TTN.EMailHandler(fuelConfig); h1.DownloadFiles();
            // отправка успешно загруженных документов на АЗС
            var s = new FtpSender(fuelConfig); s.Send();
            //Console.WriteLine("Загружено");
            //Console.ReadKey();
        }
    }
}
