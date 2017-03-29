using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public sealed class AssemblyGeneratorHelper
    {
        public static MethodInfo[] GetMethods(Type type)
        {
            return GetMethods(type, null);
        }

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

        public static string GenerateMethodDeclaration(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder(4096);
            builder.Append("public ");
            if (method.ReturnType == typeof(void))
                builder.Append("void ");
            else
                builder.Append(AssemblyGeneratorHelper.GetTypeName(method.ReturnType) + " ");

            builder.Append(method.Name);
            builder.Append("(");
            ParameterInfo[] prms = method.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo prm = prms[i];
                if (i != 0)
                    builder.Append(",");

                string typeName = AssemblyGeneratorHelper.GetTypeName(prm.ParameterType);
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

        public static Assembly GenerateAssembly(string assName, string[] sources, string[] refAssLocs)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.AddRange(refAssLocs);

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (ass.ManifestModule.ScopeName != "RefEmit_InMemoryManifestModule")
                    {
                        if (ass.Location != null && ass.Location != "")
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
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Info("No location of the assembly");
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
                LoggerWrapper.Logger.Info(builder.ToString());
                throw new Exception("Errors building '" + assName + "' assembly");
            }
            else
            {
                LoggerWrapper.Logger.Info("Successfully building '" + assName + "' assembly.");
                return cr.CompiledAssembly;
            }
        }
    }
}