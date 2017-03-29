using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dorado.Extensions
{
    public static class TypeDescriptorExtensions
    {
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomTypeDescriptor td) where TAttribute : Attribute
        {
            var attributes = td.GetAttributes().OfType<TAttribute>();
            return TypeExtensions.SortAttributesIfPossible(attributes);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this PropertyDescriptor pd) where TAttribute : Attribute
        {
            var attributes = pd.Attributes.OfType<TAttribute>();
            return TypeExtensions.SortAttributesIfPossible(attributes);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this PropertyDescriptor pd,
            Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            Guard.ArgumentNotNull(predicate, "predicate");

            var attributes = pd.Attributes.OfType<TAttribute>().Where(predicate);
            return TypeExtensions.SortAttributesIfPossible(attributes);
        }

        public static PropertyDescriptor GetProperty(this ICustomTypeDescriptor td, string name)
        {
            Guard.ArgumentNotEmpty(name, "name");
            return td.GetProperties().Find(name, true);
        }

        public static IEnumerable<PropertyDescriptor> GetPropertiesWith<TAttribute>(this ICustomTypeDescriptor td)
            where TAttribute : Attribute
        {
            return td.GetPropertiesWith<TAttribute>(x => true);
        }

        public static IEnumerable<PropertyDescriptor> GetPropertiesWith<TAttribute>(
            this ICustomTypeDescriptor td,
            Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            Guard.ArgumentNotNull(predicate, "predicate");

            return td.GetProperties()
                    .Cast<PropertyDescriptor>()
                    .Where(p => p.GetAttributes<TAttribute>().Any(predicate));
        }
    }
}