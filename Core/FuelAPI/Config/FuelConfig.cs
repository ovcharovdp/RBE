using System.Configuration;

namespace FuelAPI.Config
{
    public class FuelConfig : ConfigurationSection
    {
        [ConfigurationProperty("paths")]
        public PathConfig Paths
        {
            get { return (PathConfig)this["paths"]; }
        }
        [ConfigurationProperty("eMail")]
        public EmailConfig Email
        {
            get { return (EmailConfig)this["eMail"]; }
        }
        [ConfigurationProperty("ftpBUK")]
        public FtpBUKConfig FtpBUK
        {
            get { return (FtpBUKConfig)this["ftpBUK"]; }
        }
        [ConfigurationProperty("ftpSource")]
        public FtpSourceConfig FtpSource
        {
            get { return (FtpSourceConfig)base["ftpSource"]; }
        }
    }
}
