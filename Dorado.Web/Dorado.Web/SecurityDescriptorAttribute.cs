using System;

namespace Dorado.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SecurityDescriptorAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ModuleName
        {
            get;
            set;
        }

        public bool IsShowInMenu
        {
            get;
            set;
        }

        public SecurityDescriptorAttribute(string name, string description, bool isShowInMenu = true)
        {
            Name = name;
            Description = description;
            IsShowInMenu = isShowInMenu;
        }
    }
}