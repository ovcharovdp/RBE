using FuelAPI.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI.TTN
{
    public class FtpSender
    {
        FuelConfig _config;
        public FtpSender(FuelConfig config)
        {
            _config = config;
        }
        private void UploadFile(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_config.FtpBUK.Host + fileName);
            request.Proxy = new System.Net.WebProxy();
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_config.FtpBUK.User, _config.FtpBUK.Password);
            request.KeepAlive = true;
            request.UseBinary = true;
            StreamReader sourceStream = new StreamReader(_config.Paths.OutPath + fileName);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
        public void Send()
        {
            string[] files = Directory.GetFiles(_config.Paths.OutPath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                FileInfo fileInf = new FileInfo(file);
                UploadFile(fileInf.Name);
                fileInf.Delete();
            }
        }
    }
}
