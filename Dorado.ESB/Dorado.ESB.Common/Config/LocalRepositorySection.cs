using System;
using System.Configuration;

namespace Dorado.ESB.Common.Config
{
    #region LocalRepositorySection

    public class LocalRepositorySection : ConfigurationSection
    {
        [ConfigurationProperty("enable", DefaultValue = "true", IsRequired = false)]
        public bool Enable
        {
            get { return (bool)this["enable"]; }
            set { this["enable"] = value; }
        }

        [ConfigurationProperty("endpointName", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MaxLength = 128)]
        public string EndpointName
        {
            get { return (string)this["endpointName"]; }
            set { this["endpointName"] = value; }
        }

        [ConfigurationProperty("HostMetadata", IsRequired = true)]
        public HostMetadataElement HostMetadata
        {
            get { return (HostMetadataElement)this["HostMetadata"]; }
            set { this["HostMetadata"] = value; }
        }
    }

    public class HostMetadataElement : ConfigurationElement
    {
        public string MachineName { get { return Environment.MachineName; } }

        public string ApplicationName { get { return AppDomain.CurrentDomain.FriendlyName; } }

        [ConfigurationProperty("hostName", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MaxLength = 128)]
        public string HostName
        {
            get { return (string)this["hostName"]; }
            set { this["hostName"] = value; }
        }

        [ConfigurationProperty("assemblyNames", IsRequired = false)]
        public string AssemblyNames
        {
            get { return (string)this["assemblyNames"]; }
            set { this["assemblyNames"] = value; }
        }

        [ConfigurationProperty("Services", IsRequired = false, IsDefaultCollection = false), ConfigurationCollection(typeof(ServiceMetadaCollection))]
        public ServiceMetadaCollection Services
        {
            get { return this["Services"] as ServiceMetadaCollection; }
        }
    }

    public class ServiceMetadaCollection : AnyElementCollection<ServiceMetadataElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceMetadataElement)element).Name;
        }
    }

    public class ServiceMetadataElement : ConfigurationElement
    {
        public string Config { get; set; }

        [ConfigurationProperty("name", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MaxLength = 128)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("serviceType", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}'\"|", MaxLength = 256)]
        public string ServiceType
        {
            get { return (string)this["serviceType"]; }
            set { this["serviceType"] = value; }
        }

        [ConfigurationProperty("wrapper", IsRequired = false)]
        public bool Wrapper
        {
            get { return (bool)this["wrapper"]; }
            set { this["wrapper"] = value; }
        }

        [ConfigurationProperty("serviceInterfaceType", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}'\"|", MaxLength = 256)]
        public string ServiceInterfaceType
        {
            get { return (string)this["serviceInterfaceType"]; }
            set { this["serviceInterfaceType"] = value; }
        }

        [ConfigurationProperty("topic", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{};'\"|", MaxLength = 256)]
        public string Topic
        {
            get { return (string)this["topic"]; }
            set { this["topic"] = value; }
        }

        [ConfigurationProperty("appDomainHostName", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}/'\"|\\", MaxLength = 256)]
        public string AppDomainHostName
        {
            get { return (string)this["appDomainHostName"]; }
            set { this["appDomainHostName"] = value; }
        }

        [ConfigurationProperty("serviceNamespace", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}/'\"|\\", MaxLength = 1024)]
        public string ServiceNameSpace
        {
            get { return (string)this["serviceNamespace"]; }
            set { this["serviceNamespace"] = value; }
        }

        [ConfigurationProperty("generateWcfServiceType", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}/'\"|\\", MaxLength = 128)]
        public string GenerateWcfServiceType
        {
            get { return (string)this["generateWcfServiceType"]; }
            set { this["generateWcfServiceType"] = value; }
        }

        [ConfigurationProperty("baseAddresses", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#%^&*()[]{}'\"|", MaxLength = 512)]
        public string BaseAddresses
        {
            get { return (string)this["baseAddresses"]; }
            set { this["baseAddresses"] = value; }
        }

        [ConfigurationProperty("assemblyFolderName", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#%^&*()[]{}'\"|", MaxLength = 512)]
        public string AssemblyFolderName
        {
            get { return (string)this["assemblyFolderName"]; }
            set { this["assemblyFolderName"] = value; }
        }

        [ConfigurationProperty("assemblyNames", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#%^&*()[]{}'\"|", MaxLength = 512)]
        public string AssemblyNames
        {
            get { return (string)this["assemblyNames"]; }
            set { this["assemblyNames"] = value; }
        }

        [ConfigurationProperty("wcfType", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#%^&*()[]{}'\"|", MaxLength = 512)]
        public string WcfType
        {
            get { return (string)this["wcfType"]; }
            set { this["wcfType"] = value; }
        }

        [ConfigurationProperty("isAuthorization", IsRequired = false)]
        public bool IsAuthorization
        {
            get { return (bool)this["isAuthorization"]; }
            set { this["isAuthorization"] = value; }
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            // workaround for restricted name of the property
            if (name == "config")
            {
                Config = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion LocalRepositorySection

    public abstract class AnyElementCollection<T> : ConfigurationElementCollection where T : class
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public T this[int index]
        {
            get { return base.BaseGet(index) as T; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value as ConfigurationElement);
            }
        }

        public void Add(T element)
        {
            base.BaseAdd(element as ConfigurationElement);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return Activator.CreateInstance<T>() as ConfigurationElement;
        }

        public T Get(string key)
        {
            return base.BaseGet(key) as T;
        }

        public virtual void Remove(T element)
        {
            string name = (string)GetElementKey(element as ConfigurationElement);
            base.BaseRemove(name);
        }

        public virtual void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public virtual void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }
    }
}