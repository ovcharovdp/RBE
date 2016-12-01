using System.Configuration;

namespace FuelAPI.Config
{
    public class FolderElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
        }
    }

    public class FolderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)element).Path;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "folder"; }
        }

        protected override bool IsElementName(string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName) || elementName != "folder")
                return false;
            return true;
        }

        public FolderElement this[int index]
        {
            get { return (FolderElement)BaseGet(index); }
        }
    }
}