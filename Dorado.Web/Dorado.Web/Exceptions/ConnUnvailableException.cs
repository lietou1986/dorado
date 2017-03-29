using System.Web.Mvc;

namespace Dorado.Web.Exceptions
{
    public class ConnUnvailableException : CoreException
    {
        public ConnUnvailableException(string connName, Controller controller)
            : this(string.Format("在Controller[{1}]的Action[{2}]中未能获取数据库链接[{0}]实例。", connName, controller.RouteData.GetRequiredString("controller"), controller.RouteData.GetRequiredString("action")))
        {
        }

        public ConnUnvailableException(string connName, ControllerBase controller)
            : this(string.Format("在ControllerBase[{1}]的Action[{2}]中未能获取数据库链接[{0}]实例。"
            , connName, controller.ControllerContext.RouteData.GetRequiredString("controller"), controller.ControllerContext.RouteData.GetRequiredString("action")
            ))
        {
        }

        public ConnUnvailableException(string message)
            : base(message)
        {
        }
    }
}