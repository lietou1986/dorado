using Dorado.Ioc;
using Dorado.Web.Extensions;
using System;
using System.Web.Mvc;

namespace Dorado.Web.Filters
{
    /// <summary>
    /// 描述Controller和Action可使用服务的能力
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UsingAttribute : ActionFilterAttribute
    {
        public UsingAttribute()
        {
            if (ServiceFactory == null)
            {
                _factoryInstance = ServiceFactoryWrapper.Instance;
            }
            else
            {
                _factoryInstance = Activator.CreateInstance(ServiceFactory) as IServiceFactory;
            }
        }
        public UsingAttribute(params Type[] types)
            : this()
        {
            ServiceTypes = types;
        }
        public Type[] ServiceTypes { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ServiceTypes != null)
            {
                ServiceContainer container = filterContext.Controller.GetContainer();
                foreach (var serviceType in ServiceTypes)
                {
                    object serviceInstance = _factoryInstance.CreateInstance(serviceType);
                    container.Add(serviceType, serviceInstance);
                }
            }
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (ServiceTypes != null)
            {
                ServiceContainer container = filterContext.Controller.GetContainer();

                foreach (var type in ServiceTypes)
                {
                    object svr = container[type];
                    IDisposable dispObj = svr as IDisposable;
                    if (dispObj != null)
                    {
                        dispObj.Dispose();
                    }
                    container.Remove(type);
                }
            }
        }
        IServiceFactory _factoryInstance;
        public Type ServiceFactory { get; set; }
    }

   
    
    
}
