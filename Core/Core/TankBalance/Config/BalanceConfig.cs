using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBalance.Config
{
    public class BalanceConfig : ConfigurationSection
    {
        //[ConfigurationProperty("paths")]
        //public PathConfig Paths
        //{
        //    get { return (PathConfig)this["paths"]; }
        //}
        [ConfigurationProperty("eMail")]
        public EmailConfig Email
        {
            get { return (EmailConfig)this["eMail"]; }
        }
    }
}
