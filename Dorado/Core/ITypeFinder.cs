using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dorado.Core
{
    /// <summary>
    /// Classes implementing this interface provide information about types
    /// to various services in the SmartStore engine.
    /// </summary>
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies(Func<Assembly, bool> predicate);

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }

    public static class ITypeFinderExtensions
    {
        public static IEnumerable<Type> FindClassesOfType<T>(this ITypeFinder finder, Func<Assembly, bool> predicate = null, bool onlyConcreteClasses = true)
        {
            return finder.FindClassesOfType(typeof(T), finder.GetAssemblies(predicate), onlyConcreteClasses);
        }

        public static IEnumerable<Type> FindClassesOfType(this ITypeFinder finder, Type assignTypeFrom, Func<Assembly, bool> predicate = null, bool onlyConcreteClasses = true)
        {
            return finder.FindClassesOfType(assignTypeFrom, finder.GetAssemblies(predicate), onlyConcreteClasses);
        }

        public static IEnumerable<Type> FindClassesOfType<T>(this ITypeFinder finder, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return finder.FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }
    }
}