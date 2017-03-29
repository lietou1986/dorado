using Dorado.Core.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web.FastViewEngine
{
    public class FastViewContext
    {
        public FastViewContext()
        {
        }

        public FastViewContext(DataArrayList data)
        {
            Data = data;
        }

        public FastViewContext(DataArray data)
        {
            DataArrayList list = new DataArrayList();
            list.Add("Model", data);
            Data = list;
        }

        public FastViewContext(IDictionary<string, object> dict)
        {
            DataArrayList list = new DataArrayList();
            DataArray data = new DataArray();
            data.AddRow();
            foreach (KeyValuePair<string, object> v in dict)
            {
                data.Columns.Add(v.Key).Set(v.Value);
            }
            list.Add("Model", data);
            Data = list;
        }

        public FastViewContext(IDictionary dict)
        {
            DataArrayList list = new DataArrayList();
            DataArray data = new DataArray();
            data.AddRow();
            foreach (object v in dict.Keys)
            {
                data.Columns.Add(v.ToString()).Set(dict[v]);
            }
            list.Add("Model", data);
            Data = list;
        }

        public DataArrayList Data { get; set; }

        public HttpContextBase HttpContext { get; set; }

        public HttpResponseBase HttpResponse { get; set; }

        public PageContext PageContext { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public TempDataDictionary TempData { get; set; }

        public RouteData RouteData { get; set; }

        public UrlHelper Url { get; set; }

        public ContextObjectFactory Factory { get; set; }
    }
}