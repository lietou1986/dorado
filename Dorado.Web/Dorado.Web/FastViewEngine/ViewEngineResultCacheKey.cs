namespace Dorado.Web.FastViewEngine
{
    internal class ViewEngineResultCacheKey
    {
        public string AreaName { get; private set; }
        public string ControllerName { get; private set; }
        public string ViewName { get; private set; }

        public ViewEngineResultCacheKey(string areaName, string controllerName, string viewName)
        {
            this.AreaName = areaName ?? string.Empty;
            this.ControllerName = controllerName ?? string.Empty;
            this.ViewName = viewName ?? string.Empty;
        }

        public override int GetHashCode()
        {
            return this.AreaName.ToLower().GetHashCode() ^ this.ControllerName.ToLower().GetHashCode()
                ^ this.ViewName.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ViewEngineResultCacheKey key = obj as ViewEngineResultCacheKey;
            if (null == key)
            {
                return false;
            }
            return key.GetHashCode() == this.GetHashCode();
        }
    }
}