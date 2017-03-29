using System.Reflection;

namespace Dorado.DataExpress.Ldo
{
    public class LdoPropertyInfo
    {
        public PropertyInfo OwnerProperty
        {
            get;
            set;
        }

        public string TypeName
        {
            get
            {
                return string.Empty;
            }
        }

        public FieldAttribute Field
        {
            get;
            set;
        }

        private LdoPropertyInfo()
        {
            this.OwnerProperty = null;
        }

        public static LdoPropertyInfo Create(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(typeof(FieldAttribute), true);
            if (attrs.Length == 0)
            {
                return null;
            }
            if (!pi.CanRead || !pi.CanWrite)
            {
                return null;
            }
            MethodInfo[] mi = pi.GetAccessors();
            if (!mi[0].IsVirtual || !mi[1].IsVirtual)
            {
                return null;
            }
            return new LdoPropertyInfo
            {
                OwnerProperty = pi,
                Field = (FieldAttribute)attrs[0]
            };
        }
    }
}