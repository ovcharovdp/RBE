using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI.Config
{
    public class FuelConfig : ConfigurationSection
    {
        [ConfigurationProperty("paths")]
        public PathConfig Paths
        {
            get
            {
                return (PathConfig)this["paths"];
            }
        }
        [ConfigurationProperty("eMail")]
        public EmailConfig Email
        {
            get
            {
                return (EmailConfig)this["eMail"];
            }
        }
        [ConfigurationProperty("ftpBUK")]
        public FtpBUKConfig FtpBUK
        {
            get
            {
                return (FtpBUKConfig)this["ftpBUK"];
            }
        }
    }
}
