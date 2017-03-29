using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dorado.ESB.ServiceGenerateFactory
{
    /// <summary>
    /// host类型枚举
    /// </summary>
    public enum ServiceModelType
    {
        ServiceHost,
        WebServiceHost,
        UnKnown
    }

    public enum Forwarding
    {
        Implement,
        Interface, All
    }

    /// <summary>
    /// 动态生成host类型接口
    /// </summary>
    public interface IGenerateService
    {
        Type GetGenerateServiceType(ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes);

        Assembly GetGenerateServiceAssemblyForwarding(Forwarding forwarding, ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes, bool isAuthorization, string name);
    }
}