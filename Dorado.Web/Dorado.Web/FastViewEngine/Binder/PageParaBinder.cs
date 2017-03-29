using Dorado.Core;
using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine.Binder
{
    public class SearchParaBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction) return controllerContext.RouteData.Values[bindingContext.ModelName];

            string input = controllerContext.HttpContext.GetHttpInput();

            int startIndex = input.IndexOf("<para>");
            int endIndex = input.IndexOf("</para>");
            input = input.Substring(startIndex, endIndex - startIndex + 7).Replace("<para>", "<SearchPara>").Replace("</para>", "</SearchPara>");
            return XmlSerializerWrapper<SearchPara>.Import(input);
        }
    }
}