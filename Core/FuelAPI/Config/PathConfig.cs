using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelAPI.Config
{
    public class PathConfig : ConfigurationElement
    {
        [ConfigurationProperty("out", IsRequired = true)]
        public string OutPath
        {
            get
            {
                return (string)this["out"];
            }
        }
        [ConfigurationProperty("error", IsRequired = true)]
        public string ErrorPath
        {
            get
            {
                return (string)this["error"];
            }
        }
    }
}
