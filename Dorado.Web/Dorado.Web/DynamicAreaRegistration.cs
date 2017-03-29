using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Web.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dorado.Web
{
    public static class DynamicAreaRegistration
    {
        public static void RegisterAllAreas()
        {
            IEnumerable<Type> areaRegistrationTypes = GetTypesFromConfig(BaseConfig<GlobalWebConfiguration>.Instance);
            RegisterAreas(areaRegistrationTypes, true);
        }

        public static void RegisterAreas(params string[] assemblyNames)
        {
            IEnumerable<Type> areaRegistrationTypes = GetAreadRegistrationTypes(assemblyNames);
            RegisterAreas(areaRegistrationTypes, true);
        }

        internal static void RegisterAreas(IEnumerable<Type> foundTypes, bool findReferencedAssemblies)
        {
            ICollection referencedAssemblies = BuildManager.GetReferencedAssemblies();
            List<Type> areaRegistrationTypes = new List<Type>();
            if (findReferencedAssemblies)
            {
                foreach (Assembly assembly in referencedAssemblies)
                {
                    try
                    {
                        areaRegistrationTypes.AddRange(assembly.GetTypes().Where(IsAreaRegistrationType));
                    }
                    catch
                    {
                    }
                }
            }
            if (foundTypes != null)
            {
                areaRegistrationTypes.AddRange(
                    from foundType in foundTypes
                    where !areaRegistrationTypes.Contains(foundType)
                    select foundType);
            }
            RegisterAreaFromTypes(areaRegistrationTypes);
        }

        private static void RegisterAreaFromTypes(IEnumerable<Type> areaRegistrationTypes)
        {
            if (areaRegistrationTypes != null)
            {
                foreach (Type registrationType in areaRegistrationTypes)
                {
                    AreaRegistration area = (AreaRegistration)Activator.CreateInstance(registrationType);
                    AreaRegistrationContext areaRegistrationContext = new AreaRegistrationContext(area.AreaName, RouteTable.Routes);
                    string thisNamespace = area.GetType().Namespace;
                    if (thisNamespace != null)
                    {
                        areaRegistrationContext.Namespaces.Add(thisNamespace + ".*");
                    }
                    area.RegisterArea(areaRegistrationContext);
                }
            }
        }

        private static IEnumerable<Type> GetTypesFromConfig(GlobalWebConfiguration config)
        {
            if (config.RegisteredAssemblies == null)
                return null;
            List<string> assemblies = config.RegisteredAssemblies;
            return GetAreadRegistrationTypes(assemblies.ToArray());
        }

        private static IEnumerable<Type> GetAreadRegistrationTypes(params string[] assemblies)
        {
            List<Type> types = new List<Type>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                string registeredAssemblies = assemblies[i];
                IEnumerable<Type> typesInAssembly = GetTypesFromAssembly(registeredAssemblies);
                types.AddRange(typesInAssembly);
            }
            return types;
        }

        private static IEnumerable<Type> GetTypesFromAssembly(string assemblyName)
        {
            List<Type> findTypes = new List<Type>();
            if (string.IsNullOrEmpty(assemblyName))
            {
                LoggerWrapper.Logger.Error("assembly name can't be null.", new ArgumentNullException(assemblyName, "assembly name cann't be null."));
                return null;
            }
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                Type[] types = assembly.GetTypes();
                findTypes.AddRange(types.Where(IsAreaRegistrationType));
            }
            catch (ArgumentException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileLoadException)
            {
            }
            catch (BadImageFormatException)
            {
            }
            catch (Exception)
            {
            }
            return findTypes;
        }

        private static bool IsAreaRegistrationType(Type type)
        {
            return type.IsPublic && !type.IsAbstract && typeof(AreaRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}