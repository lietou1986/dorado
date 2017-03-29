using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Dorado.ESB.GenerateServices
{
    public class GenerateWebServiceHost
    {
        private static string WcfNameSpace = "";

        private static string GetTypeName(Type type)
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

        private static MethodInfo[] GetMethods(Type type)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            List<MethodInfo> ret = new List<MethodInfo>();
            foreach (MethodInfo info in infos)
            {
                //object[] attrs = info.GetCustomAttributes(typeof(JsonAttribute), true);
                object[] attrsWebGet = info.GetCustomAttributes(typeof(WebGetAttribute), true);
                object[] attrsWebInvoke = info.GetCustomAttributes(typeof(WebInvokeAttribute), true);
                if (attrsWebGet.Length != 0 || attrsWebInvoke.Length != 0)
                    ret.Add(info);
            }
            Type[] interfaces = type.GetInterfaces();
            foreach (Type itf in interfaces)
            {
                MethodInfo[] subInfos = GetMethods(itf);
                foreach (MethodInfo info in subInfos)
                {
                    if (!ret.Contains(info))
                        ret.Add(info);
                }
            }
            return ret.ToArray();
        }

        private static string GetServiceTypeCode(Type serviceType)
        {
            MethodInfo[] methods = GetMethods(serviceType);
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.ServiceModel.Web;");

            //----------------------------------------------------------
            //Add By jiangsong 2009-11-14
            builder.AppendLine("using Dorado.PlatformServices.Common.DataContracts;");

            //----------------------------------------------------------

            builder.AppendLine("using Dorado.ESB.GenerateServices;");

            builder.AppendLine("namespace " + WcfNameSpace + "{");

            builder.AppendLine("[ServiceContract]");
            builder.AppendLine("public class " + serviceType.Name + " : ApiWcfBase {");
            foreach (MethodInfo method in methods)
            {
                builder.AppendLine("[OperationContract]");

                object[] attrsWebGet = method.GetCustomAttributes(typeof(WebGetAttribute), true);
                object[] attrsWebInvoke = method.GetCustomAttributes(typeof(WebInvokeAttribute), true);

                if (attrsWebGet.Length > 0)
                {
                    WebGetAttribute attr = attrsWebGet[0] as WebGetAttribute;

                    builder.AppendLine("[WebGet(UriTemplate=\"" + attr.UriTemplate + "\",ResponseFormat=WebMessageFormat.Json)]");
                }

                if (attrsWebInvoke.Length > 0)
                {
                    WebInvokeAttribute attr = attrsWebInvoke[0] as WebInvokeAttribute;
                    builder.AppendLine("[WebInvoke(Method=\"POST\", BodyStyle= WebMessageBodyStyle.WrappedRequest, UriTemplate = \"" + attr.UriTemplate + "\",RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]");
                }

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
                    if (prm.IsOut)
                    {
                        string typeName = GetTypeName(prm.ParameterType);
                        typeName = typeName.Substring(0, typeName.Length - 1);
                        builder.Append("out " + typeName + " " + prm.Name);
                    }
                    else
                        builder.Append(GetTypeName(prm.ParameterType) + " " + prm.Name);

                    //builder.Append(GetTypeName(prm.ParameterType) + " " + prm.Name);
                }
                builder.AppendLine(")");
                builder.AppendLine("{");

                //----------------------------------------------------------
                //Del By jiangsong 2009-11-14
                //builder.AppendLine("using (new ContextScope()){");
                //----------------------------------------------------------
                builder.AppendLine("try{");
                if (method.ReturnType != typeof(void))
                    builder.Append("return ");
                builder.Append("new " + serviceType.FullName + "()." + method.Name + "(");
                for (int i = 0; i < prms.Length; i++)
                {
                    ParameterInfo prm = prms[i];
                    if (i != 0)
                        builder.Append(",");
                    if (prm.IsOut)
                        builder.Append("out " + prm.Name);
                    else
                        builder.Append(prm.Name);

                    //builder.Append(prm.Name);
                }
                builder.AppendLine(");");

                string returnValue = "";
                if (method.ReturnType == typeof(void))
                {
                    returnValue = "";
                }
                else if (method.ReturnType == typeof(int) || method.ReturnType == typeof(long))
                {
                    returnValue = "return -1;";
                }
                else if (method.ReturnType == typeof(bool))
                {
                    returnValue = "return false;";
                }
                else
                {
                    returnValue = "return null;";
                }

                builder.AppendLine(@"}
                    catch (Exception ex)
                    {
                         ApiWcfBase.LogException(ex, ""Dorado.ESB.Server"");
                    ");
                builder.AppendLine(returnValue);
                builder.AppendLine("}");

                //----------------------------------------------------------
                //Del By jiangsong 2009-11-14
                //builder.AppendLine("}");
                //----------------------------------------------------------
                builder.AppendLine("}");
            }
            builder.AppendLine("}");
            builder.AppendLine("}");

            //----------------------------Add By jiangsong 2009-11-14

            //string path = @"C:\";
            //System.IO.File.WriteAllText(path+"GenerateWebServiceHost-GetServiceTypeCode-" + System.Guid.NewGuid().ToString() + ".txt", builder.ToString());

            //----------------------------

            return builder.ToString();
        }

        private static Assembly GetJsonServiceTypeAssembly(Type serviceType)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(WebGetAttribute).Assembly.Location);

            //----------------------------------------------------------
            //remove Dorado.PlatformServices.Interface Namespace By jiangsong 2009-11-14
            //cp.ReferencedAssemblies.Add(typeof(Dorado.PlatformServices.Common.DataContracts.BaseDataContract).Assembly.Location);
            //----------------------------------------------------------

            cp.ReferencedAssemblies.Add(serviceType.Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(Dorado.ESB.GenerateServices.ApiWcfBase).Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(LoggerWrapper).Assembly.Location);

            //cp.ReferencedAssemblies.Add(this.GetType().Assembly.Location);
            //----------------------------------------------------------
            //remove Dorado.Entity Namespace By jiangsong 2009-11-14
            //cp.ReferencedAssemblies.Add(typeof(Dorado.Entity.ContextScope).Assembly.Location);
            //----------------------------------------------------------
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            string code = GetServiceTypeCode(serviceType);

            //if (!Dorado.Configuration.MaintenanceConfig.FunctionIsDisabled(1, "JsonStubOutput"))
            //{
            //    try
            //    {
            //        System.IO.File.WriteAllText(@"c:\platform_service_webhttp_wcf.cs", code);
            //    }
            //    catch (Exception ex)
            //    {
            //        LoggerWrapper.Logger.Error(ex, "SelfDescribingWebServices");
            //    }
            //}
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, code);
            if (cr.Errors.Count > 0)
            {
                StringBuilder builder = new StringBuilder(4096);
                builder.AppendLine("Errors building '" + WcfNameSpace + "' assembly:");
                foreach (CompilerError ce in cr.Errors)
                {
                    builder.AppendLine("  " + ce.ToString());
                    builder.AppendLine();
                }
                LoggerWrapper.Logger.Info("Dorado.ESB.GenerateServices:" + builder.ToString());
                throw new Exception("Errors building '" + WcfNameSpace + "' assembly");
            }
            else
            {
                LoggerWrapper.Logger.Info("Successfully building '" + WcfNameSpace + "' assembly.");
                return cr.CompiledAssembly;
            }
        }

        private static Type GenerateWcfServiceType(Type serviceType)
        {
            Assembly ass = GetJsonServiceTypeAssembly(serviceType);
            return ass.GetType(WcfNameSpace + "." + serviceType.Name);
        }

        private static Type wcfServiceType;

        public static Type GetWcfServiceType(Type serviceType, string serviceNamespace)
        {
            if (wcfServiceType == null)
            {
                if (serviceNamespace == null || serviceNamespace == "")
                    WcfNameSpace = serviceType.FullName;
                else
                    WcfNameSpace = serviceNamespace;

                wcfServiceType = GenerateWcfServiceType(serviceType);
            }
            return wcfServiceType;
        }
    }
}