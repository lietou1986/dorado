using Dorado.Core.Data;
using Dorado.Extensions;
using System.Web.Mvc;

namespace Dorado.Web.FastViewEngine.Binder
{
    public class DataRequireAttribute : CustomModelBinderAttribute
    {
        private string[] Requires { get; set; }

        public DataRequireAttribute(string[] requires)
        {
            this.Requires = requires;
        }

        public DataRequireAttribute(string requires)
        {
            this.Requires = requires.Split(new[] { ',', '|' });
        }

        public override IModelBinder GetBinder()
        {
            return new DataArrayBinder(Requires);
        }
    }

    public class DataArrayBinder : IModelBinder
    {
        private string[] Requires { get; set; }

        public DataArrayBinder()
        {
        }

        public DataArrayBinder(string[] requires)
        {
            this.Requires = requires;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.IsChildAction) return controllerContext.RouteData.Values[bindingContext.ModelName];

            string dataContent = controllerContext.HttpContext.GetHttpInput();

            DataArray data = new DataArray();
            data.ReadXml(dataContent);
            if (Requires != null && Requires.Length > 0)
            {
                foreach (string v in Requires)
                {
                    if (!data.Contains(v))
                        throw new CoreException("数据无效，没有找到数据：{0}", v);
                }
            }
            return data;
        }
    }
}