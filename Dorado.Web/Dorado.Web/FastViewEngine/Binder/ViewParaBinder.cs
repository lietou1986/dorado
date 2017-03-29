using Dorado.Core;
using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine.Binder
{
    public class ViewParaBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction) return controllerContext.RouteData.Values[bindingContext.ModelName];

            string input = controllerContext.HttpContext.GetHttpInput();

            int startIndex = input.IndexOf("<para>");
            int endIndex = input.IndexOf("</para>");
            input = input.Substring(startIndex, endIndex - startIndex + 7).Replace("<para>", "<ViewPara>").Replace("</para>", "</ViewPara>");
            return XmlSerializerWrapper<ViewPara>.Import(input);
        }
    }
}