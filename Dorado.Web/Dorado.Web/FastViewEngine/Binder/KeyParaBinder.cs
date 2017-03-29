using Dorado.Core;
using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine.Binder
{
    public class KeyParaBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction) return controllerContext.RouteData.Values[bindingContext.ModelName];

            string input = controllerContext.HttpContext.GetHttpInput();

            int startIndex = input.IndexOf("<id>");
            int endIndex = input.IndexOf("</id>");
            input = input.Substring(startIndex, endIndex - startIndex + 5).Replace("<id>", "<KeyPara><id>").Replace("</id>", "</id></KeyPara>");
            return XmlSerializerWrapper<KeyPara>.Import(input);
        }
    }
}