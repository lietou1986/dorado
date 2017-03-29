using Dorado.ActivityEngine.ServiceInterface;
using System;
using System.Reflection;

namespace Dorado.ActivityEngine.ServiceImp
{
    internal class EsbInterfaceHelper
    {
        public static EsbInterfaceCaller GetEsbInterfaceCaller(string interfaceFullName, string methodName)
        {
            Type interfaceType = Type.GetType(interfaceFullName);
            if (interfaceType == null)
            {
                throw new TypeNotFoundException(interfaceFullName);
            }
            Type genericFactoryType = typeof(EsbInterfaceHelper);
            Type factoryType = genericFactoryType.MakeGenericType(new Type[]
			{
				interfaceType
			});
            MethodInfo createMethod = factoryType.GetMethod("CreateInstance", Type.EmptyTypes);
            object esbClient = createMethod.Invoke(null, null);
            if (esbClient == null)
            {
                throw new ActivityEngineException("Error creating client for ESB interface " + interfaceFullName);
            }
            MethodInfo esbMethod = esbClient.GetType().GetMethod(methodName, new Type[]
			{
				typeof(Activity[])
			});
            if (esbMethod == null)
            {
                throw new ActivityEngineException(string.Format("Method '{0}' not found in ESB interface '{1}', the method must accept 1 argument of type Activity[] and return void", methodName, interfaceFullName));
            }
            return delegate(Activity[] activies)
            {
                esbMethod.Invoke(esbClient, new object[]
				{
					activies
				});
            }
            ;
        }
    }
}