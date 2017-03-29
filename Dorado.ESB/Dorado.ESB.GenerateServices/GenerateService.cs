using Dorado.ESB.ServiceGenerateFactory;
using System;
using System.Collections.Generic;

namespace Dorado.ESB.GenerateServices
{
    public class GenerateService : MarshalByRefObject, IGenerateService
    {
        //2010/05/18
        public Type GetGenerateServiceType(ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes)
        {
            switch (serviceModelType)
            {
                case ServiceModelType.ServiceHost:
                    return GenerateServiceHost.GetWcfServiceType(listTypes, serviceNamespaces);

                case ServiceModelType.WebServiceHost:

                //return GenerateWebServiceHost.GetWcfServiceType(type, serviceNamespaces);
                default:
                    return null;
            }
        }

        public System.Reflection.Assembly GetGenerateServiceAssemblyForwarding(Forwarding forwarding, ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes, bool isAuthorization, string name)
        {
            return GenerateServiceForwarding.GetGenerateServiceAssemblyForwarding(forwarding, serviceModelType, serviceNamespaces, listTypes, isAuthorization, name);
        }
    }
}