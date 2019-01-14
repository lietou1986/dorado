using System;
using System.Collections.Generic;

namespace Dorado.Ioc
{
    /// <summary>
    /// 服务容器类
    /// </summary>
    public class ServiceContainer : Dictionary<Type, object>
    {
        public static class Consts
        {
            public const string ServiceContainerName = "ServiceContainer";
        }
    }
}