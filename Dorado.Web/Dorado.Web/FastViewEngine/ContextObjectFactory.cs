using System.Web.Routing;

namespace Dorado.Web.FastViewEngine
{
    public class ContextObjectFactory
    {
        public RouteValueDictionary CreateRouteData()
        {
            return new RouteValueDictionary();
        }

        public string TypeTest(object param)
        {
            return param.GetType().ToString();
        }

        public RouteValueDictionary CreateRouteData(params string[] args)
        {
            RouteValueDictionary dic = CreateRouteData();
            int max = args.Length / 2;
            for (int i = 0; i < max; i++)
            {
                int idx = i * 2;
                dic.Add(args[idx], args[idx + 1]);
            }
            return dic;
        }
    }
}