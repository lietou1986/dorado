using System;
using System.Web.Mvc;

namespace Dorado.Web.ModelBinder
{
    public class StringArrayAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new StringArrayBinder();
        }
    }

    public class StringArrayBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = controllerContext.HttpContext.Request[bindingContext.ModelName];
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(bindingContext.ModelName);
            return value.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}