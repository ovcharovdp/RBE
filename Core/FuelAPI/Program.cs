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
            System.IO.StreamWriter file = new System.IO.StreamWriter(".\\Logs\\" + DateTime.Today.ToString("yyyy_MM_dd") + ".log", true);
            file.WriteLine("*** " + DateTime.Now.ToShortTimeString() + " ***");
            try
            {
                new Logistica.LogisticaChuv(file).UploadWaybills();
            }
            catch (Exception e)
            {
                file.WriteLine("Чуваши:" + e.Message);
            }
            // загрузка плана
            try
            {
                new Logistica.Logistica(file).UploadWaybills();
            }
            catch (Exception e)
            {
                file.WriteLine("Логистика:" + e.Message);
            }
            // загрузка факта с FTP
            try
            {
                var h = new TTN.Handler(fuelConfig); h.Handle();
            }
            catch (Exception e)
            {
                file.WriteLine("FTP:" + e.Message);
            }
            // загрузка факта с почты
            try
            {
                var h1 = new TTN.EMailHandler(fuelConfig); h1.DownloadFiles();
            }
            catch (Exception e)
            {
                file.WriteLine("eMail:" + e.Message);
            }
            try
            {
                // отправка успешно загруженных документов на АЗС
                var s = new FtpSender(fuelConfig); s.Send();
            }
            catch (Exception e)
            {
                file.WriteLine("FTPSender:" + e.Message);
            }
            file.Close();
            //Console.WriteLine("Загружено");
            //Console.ReadKey();
        }
    }
}
