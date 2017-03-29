using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dorado.Extensions
{
    /// <summary>
    /// 反射工具类
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class ReflectorExtension
    {
        /// <summary>
        /// 判断指定的类型是否派生自某接口
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public static bool IsImplementsInterface(this Type objectType, Type interfaceType)
        {
            if (objectType == null || interfaceType == null || !interfaceType.IsInterface)
                return false;

            return ((IList<Type>)objectType.GetInterfaces()).Contains(interfaceType);
        }

        private static readonly Type[] _EmptyTypeArray = new Type[0];

        /// <summary>
        /// 判断该类型是否可以通过new直接创建
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns></returns>
        public static bool CanCreateDirect(this Type objectType)
        {
            Contract.Requires(objectType != null);

            return !objectType.IsAbstract && !objectType.IsArray && !objectType.IsInterface && !objectType.IsPointer
                && !objectType.IsGenericTypeDefinition && !objectType.IsEnum && objectType.HasDefaultConstructor();
        }

        /// <summary>
        /// 获取指定类型的指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type objectType, bool inherit = false)
            where T : Attribute
        {
            Contract.Requires(objectType != null);

            T[] attrs = (T[])objectType.GetCustomAttributes(typeof(T), inherit);
            return attrs.Length > 0 ? attrs[0] : null;
        }

        /// <summary>
        /// 获取指定类型的所有指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this Type objectType, bool inherit = false)
            where T : Attribute
        {
            Contract.Requires(objectType != null);

            return (T[])objectType.GetCustomAttributes(typeof(T), inherit);
        }

        /// <summary>
        /// 获取指定类型的指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false)
            where T : Attribute
        {
            Contract.Requires(memberInfo != null);

            T[] attrs = (T[])memberInfo.GetCustomAttributes(typeof(T), inherit);
            return attrs.Length > 0 ? attrs[0] : null;
        }

        /// <summary>
        /// 获取指定成员的所有指定Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false)
            where T : Attribute
        {
            Contract.Requires(memberInfo != null);

            return (T[])memberInfo.GetCustomAttributes(typeof(T), inherit);
        }

        /// <summary>
        /// 判断指定的方法是否与指定的委托相匹配
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        public static bool IsMatch(this MethodInfo methodInfo, Type delegateType)
        {
            Contract.Requires(methodInfo != null && delegateType != null && delegateType.IsSubclassOf(typeof(Delegate)));

            MethodInfo mInfo = delegateType.GetMethod("Invoke");
            ParameterInfo[] params1, params2;
            if ((params1 = methodInfo.GetParameters()).Length != (params2 = mInfo.GetParameters()).Length)
                return false;

            for (int k = 0, length = params1.Length; k < length; k++)
            {
                Type type1 = params1[k].ParameterType, type2 = params2[k].ParameterType;
                if (type1 != type2 && !type2.IsSubclassOf(type1))
                    return false;
            }

            Type returnType1 = methodInfo.ReturnType, returnType2 = mInfo.ReturnType;
            return returnType1 == returnType2 || returnType1.IsSubclassOf(returnType2);
        }

        public static T GetAttribute<T>(this Enum e) where T : Attribute
        {
            Type type = typeof(T);
            FieldInfo fi = e.GetType().GetField(e.ToString());
            if (fi == null)
                return Activator.CreateInstance(type) as T;
            else
            {
                T[] attributes = (T[])fi.GetType().GetCustomAttributes(type, false);
                return attributes.Length > 0 ? attributes[0] : Activator.CreateInstance(type) as T;
            }
        }

        /// <summary>
        /// 枚举转换成字典集合
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IDictionary<int, string> ToList(this Enum e)
        {
            IDictionary<int, string> dict = new Dictionary<int, string>();
            Type type = e.GetType();
            FieldInfo[] fis = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            if (fis == null || fis.Length == 0)
                return dict;
            else
            {
                foreach (FieldInfo fi in fis)
                {
                    object[] objs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (objs != null && objs.Length > 0)
                        dict.Add((int)Enum.Parse(type, fi.Name), (objs[0] as DescriptionAttribute).Description);
                    else
                        dict.Add((int)Enum.Parse(type, fi.Name), fi.Name);
                }
            }
            return dict.OrderBy(n => n.Key).ToDictionary(n => n.Key, n => n.Value);
        }

        #region 程序集生成操作

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(Type type)
        {
            return GetMethods(type, null);
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attrType">Type of the attr.</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(Type type, Type attrType)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            List<MethodInfo> ret = new List<MethodInfo>();
            foreach (MethodInfo info in infos)
            {
                if (attrType != null)
                {
                    object[] attrs = info.GetCustomAttributes(attrType, true);
                    if (attrs.Length != 0)
                        ret.Add(info);
                }
                else
                    ret.Add(info);
            }
            Type[] interfaces = type.GetInterfaces();
            foreach (Type itf in interfaces)
            {
                ret.AddRange(GetMethods(itf, attrType));
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(type.FullName.Substring(0, type.FullName.IndexOf('`')));
                builder.Append("<");
                Type[] types = type.GetGenericArguments();
                for (int i = 0; i < types.Length; i++)
                {
                    if (i != 0)
                        builder.Append(",");
                    builder.Append(GetTypeName(types[i]));
                }
                builder.Append(">");
                return builder.ToString();
            }
            else
                return type.FullName;
        }

        /// <summary>
        /// Generates the method declaration.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static string GenerateMethodDeclaration(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder(4096);
            builder.Append("public ");
            if (method.ReturnType == typeof(void))
                builder.Append("void ");
            else
                builder.Append(GetTypeName(method.ReturnType) + " ");

            builder.Append(method.Name);
            builder.Append("(");
            ParameterInfo[] prms = method.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo prm = prms[i];
                if (i != 0)
                    builder.Append(",");

                string typeName = GetTypeName(prm.ParameterType);
                if (prm.IsOut)
                {
                    builder.Append("out ");
                    if (typeName[typeName.Length - 1] == '&')
                        typeName = typeName.Substring(0, typeName.Length - 1);
                }
                builder.Append(typeName + " " + prm.Name);
            }
            builder.AppendLine(")");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the method parameter string.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static string GetMethodParameterString(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder(1024);
            ParameterInfo[] prms = method.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo prm = prms[i];
                if (i != 0)
                    builder.Append(",");
                if (prm.IsOut)
                    builder.Append("out ");
                builder.Append(prm.Name);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates the assembly.
        /// </summary>
        /// <param name="assName">Name of the ass.</param>
        /// <param name="sources">The sources.</param>
        /// <param name="refAssLocs">The ref ass locs.</param>
        /// <param name="generateInMemory">if set to <c>true</c> [generate in memory].</param>
        /// <returns></returns>
        public static Assembly GenerateAssembly(string assName, string[] sources, string[] refAssLocs, bool generateInMemory = true)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = generateInMemory;
            if (!generateInMemory)
                cp.OutputAssembly = assName + ".dll";
            cp.CompilerOptions = "/optimize";
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.AddRange(refAssLocs);

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.ManifestModule.ScopeName != "RefEmit_InMemoryManifestModule")
                {
                    if (ass.Location != null && ass.Location != string.Empty)
                    {
                        if (!ass.Location.Contains("System.dll"))
                        {
                            if (!cp.ReferencedAssemblies.Contains(ass.Location))
                            {
                                cp.ReferencedAssemblies.Add(ass.Location);
                            }
                        }
                    }
                }
            }

            IDictionary<string, string> version = new Dictionary<string, string>();
            if (System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().StartsWith("v4.0"))
                version.Add("CompilerVersion", "v4.0");
            else
                version.Add("CompilerVersion", "v3.5");
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(version);
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, sources);

            if (cr.Errors.Count > 0)
            {
                StringBuilder builder = new StringBuilder(4096);
                builder.AppendLine("Errors building '" + assName + "' assembly:");
                foreach (CompilerError ce in cr.Errors)
                {
                    builder.AppendLine("  " + ce.ToString());
                    builder.AppendLine();
                }

                throw new Exception("Errors building '" + assName + "' assembly");
            }
            else
            {
                return cr.CompiledAssembly;
            }
        }

        /// <summary>
        /// Defaults for type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// Defaults the type of the string for.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static string DefaultStringForType(Type targetType)
        {
            string typeString = targetType.FullName;
            if (typeString.Contains("&"))
            {
                string rawTypeString = typeString.Replace("&", string.Empty);
                targetType = Assembly.GetAssembly(typeof(Int32)).GetType(rawTypeString);
            }
            if (targetType == typeof(Boolean))
                return "false";
            else
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType).ToString() : "null";
            }
        }

        #endregion 程序集生成操作
    }
}