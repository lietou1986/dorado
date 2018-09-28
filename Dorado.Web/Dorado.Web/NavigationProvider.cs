using Dorado.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Dorado.Web
{
    public static class NavigationProvider
    {
        private static Dictionary<string, List<NavigationDescription>> _allNavigation;

        public static Dictionary<string, List<NavigationDescription>> AllNavigation
        {
            get
            {
                return _allNavigation ?? GetAllNavigation();
            }
        }

        internal static Dictionary<string, List<NavigationDescription>> GetAllNavigation()
        {
            Dictionary<string, List<NavigationDescription>> dict = new Dictionary<string, List<NavigationDescription>>();
            List<Assembly> assemblies = (
                from asm in AppDomain.CurrentDomain.GetAssemblies()
                where asm.FullName.StartsWith("Dorado", StringComparison.OrdinalIgnoreCase)
                select asm).ToList();
            List<AreaDescription> allAreas = (
                from area in assemblies.SelectMany((Assembly m) => m.GetTypes())
                where area.IsSubclassOf(typeof(AreaRegistration)) && !area.IsAbstract
                select new AreaDescription
                {
                    AreaInstance = (AreaRegistration)Activator.CreateInstance(area),
                    AreaType = area
                }).ToList();
            IEnumerable<NavigationDescription> allNavigation =
                from m in
                    (
                        from type in
                            (
                                from asm in assemblies
                                select asm).SelectMany((Assembly a) => a.GetTypes())
                        where type.IsPublic && type.IsSubclassOf(typeof(Controller))
                        select type).SelectMany((Type t) => t.GetMethods())
                where m.ReturnType.IsSubclassOf(typeof(Result)) || m.ReturnType == typeof(Result)
                select new NavigationDescription
                {
                    Action = m.Name,
                    Controller = m.DeclaringType.Name.Substring(0, m.DeclaringType.Name.Length - 10),
                    Area = ((
                                from a in allAreas
                                where a.AreaInstance.AreaName == m.DeclaringType.Namespace.Substring(m.DeclaringType.Namespace.LastIndexOf(".") + 1)
                                select a).Any()) ? (
                        from a in allAreas
                        where a.AreaInstance.AreaName == m.DeclaringType.Namespace.Substring(m.DeclaringType.Namespace.LastIndexOf(".") + 1)
                        select a).First().AreaInstance.AreaName : "NoArea",
                    SecurityDescriptor = ((m.GetCustomAttributes(typeof(SecurityDescriptorAttribute), false).Length > 0) ? m.GetCustomAttributes(typeof(SecurityDescriptorAttribute), false).First() : null) as SecurityDescriptorAttribute,
                    HttpMethod = (m.GetCustomAttributes(typeof(HttpPostAttribute), false).Length > 0) ? "POST" : "GET"
                };
            var group = from g in allNavigation
                        group g by g.Area into g
                        select new
                        {
                            Key = g.Key,
                            Values = g.ToList()
                        };
            foreach (var g in group)
            {
                dict[g.Key] = g.Values;
            }
            _allNavigation = dict;
            return dict;
        }
    }
}