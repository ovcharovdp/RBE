using System.Configuration;

namespace FuelAPI.Config
{
    public class FtpSourceConfig : ConfigurationElement
    {
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

        [ConfigurationProperty("", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
        public FolderCollection Folders
        {
            get { return (FolderCollection)base[""]; }
        }
    }
}
