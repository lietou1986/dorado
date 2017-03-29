using Dorado.ActivityEngine.ServiceInterface;
using System.Xml;

namespace Dorado.ActivityEngine.ServiceImp
{
    public static class ActivityFilterFactory
    {
        public static IActivityFilter CreateFilter(string filterType, XmlElement filterConfig)
        {
            if (filterType != null)
            {
                if (filterType == "ActivityGroupFilter")
                {
                    return new ActivityGroupFilter(filterConfig);
                }
                if (filterType == "ActivityTypeFilter")
                {
                    return new ActivityTypeFilter(filterConfig);
                }
                if (filterType == "AllowAllFilter")
                {
                    return new AllowAllFilter();
                }
                if (filterType == "DenyAllFilter")
                {
                    return new DenyAllFilter();
                }
            }
            throw new UnknownActivityFilterException(filterType);
        }
    }
}