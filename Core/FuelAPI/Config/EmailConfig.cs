using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI.Config
{
    public class EmailConfig : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
        }
        [ConfigurationProperty("port", IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)this["port"];
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
