namespace Dorado.Web.Model
{
    public class NavigationDescription
    {
        public string Area
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }

        public string Controller
        {
            get;
            set;
        }

        public string HttpMethod
        {
            get;
            set;
        }

        public SecurityDescriptorAttribute SecurityDescriptor
        {
            get;
            set;
        }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(Area))
                    return string.Format("{0}_{1}_{2}", Area, Controller, Action);
                return string.Format("{0}_{1}", Controller, Action);
            }
        }

        public string DisplayName
        {
            get
            {
                if (SecurityDescriptor != null)
                    return SecurityDescriptor.Name;
                return string.Empty;
            }
        }

        public string Description
        {
            get
            {
                if (SecurityDescriptor != null)
                    return SecurityDescriptor.Description;
                return string.Empty;
            }
        }
    }
}