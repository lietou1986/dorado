using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dorado.DataExpress.Ldo
{
    public class LdoClassInfo
    {
        private Type _compiledType;

        public Type OwnerType
        {
            get;
            set;
        }

        public TableAttribute Table
        {
            get;
            set;
        }

        public LdoPropertyInfo[] Properties
        {
            get;
            set;
        }

        public string LdoNamespace
        {
            get
            {
                return this.OwnerType.Namespace + ".Ldo";
            }
        }

        public string LdoClass
        {
            get
            {
                return this.OwnerType.Name + "Entity";
            }
        }

        public string LdoFullName
        {
            get
            {
                return this.LdoNamespace + "." + this.LdoClass;
            }
        }

        public string LdoCode
        {
            get
            {
                return string.Empty;
            }
        }

        public Type CompiledType
        {
            get
            {
                if (this._compiledType == null)
                {
                    this._compiledType = this.Compile();
                }
                return this._compiledType;
            }
        }

        private LdoClassInfo()
        {
        }

        public static LdoClassInfo Create(string typeName)
        {
            Type type = Type.GetType(typeName, true, true);
            return LdoClassInfo.Create(type);
        }

        public static LdoClassInfo Create(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (attrs.Length == 0)
            {
                return null;
            }
            LdoClassInfo clazz = new LdoClassInfo
            {
                OwnerType = type,
                Table = (TableAttribute)attrs[0]
            };
            PropertyInfo[] properties = clazz.OwnerType.GetProperties();
            List<LdoPropertyInfo> pis = new List<LdoPropertyInfo>(properties.Length);
            pis.AddRange(
                from pi in properties
                select LdoPropertyInfo.Create(pi) into lpi
                where lpi != null
                select lpi);
            clazz.Properties = pis.ToArray();
            return clazz;
        }

        protected Type Compile()
        {
            return null;
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(this.CompiledType);
        }

        public T CreateInstance<T>()
        {
            return (T)this.CreateInstance();
        }
    }
}