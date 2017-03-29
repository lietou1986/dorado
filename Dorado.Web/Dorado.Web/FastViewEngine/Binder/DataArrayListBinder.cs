using Dorado.Core.Data;
using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine.Binder
{
    public class DataArrayListBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction) return controllerContext.RouteData.Values[bindingContext.ModelName];

            string dataContent = controllerContext.HttpContext.GetHttpInput();

            DataArrayList dataList = new DataArrayList();
            dataList.ReadXml(dataContent);
            return dataList;
        }
    }
}