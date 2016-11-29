using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI.Config
{
    public class FtpBUKConfig : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
        }
        [ConfigurationProperty("userid", IsRequired = true)]
        public string User
        {
            get
            {
                return (string)this["userid"];
            }
        }
        [ConfigurationProperty("pass", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["pass"];
            }
        }
    }
}
