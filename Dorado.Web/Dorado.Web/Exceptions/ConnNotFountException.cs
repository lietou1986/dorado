using System;
using System.Web.Mvc;

namespace Dorado.Web.Exceptions
{
    public class ConnNotFountException : CoreException
    {
        public ConnNotFountException(string message)
            : base(message)
        {
        }

        public ConnNotFountException(string connName, Exception error)
            : base(string.Format("获取数据库链接发生异常[{0}]", connName), error)
        {
        }

        public ConnNotFountException(string connName, Controller controller)
            : this(string.Format("未能找到数据库链接{0},请确定在Controller[{1}]中的Action[{2}]中添加了[UsingConn({0})]标签", new object[]
		{
			connName,
			controller.GetType(),
			controller.RouteData.GetRequiredString("action"),
			connName
		}))
        {
        }

        public ConnNotFountException(string connName, ControllerBase controller)
            : this(string.Format("未能找到数据库链接{0},请确定在ControllerBase[{1}]中的Action[{2}]中添加了[Using({0})]标签"
            , connName, controller.GetType(), controller.ControllerContext.RouteData.GetRequiredString("action"), connName
            ))
        {
        }
    }
}