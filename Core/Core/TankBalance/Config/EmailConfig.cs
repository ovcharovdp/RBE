using System.Configuration;

namespace TankBalance.Config
{
    public class EmailConfig : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return (string)this["host"]; }
        }
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
        }
        [ConfigurationProperty("userid", IsRequired = true)]
        public string User
        {
            get { return (string)this["userid"]; }
        }
        [ConfigurationProperty("pass", IsRequired = true)]
        public string Password
        {
            get { return (string)this["pass"]; }
        }
    }
}