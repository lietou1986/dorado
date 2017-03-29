using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ServiceGenerateFactory;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.ESB.GenerateServices
{
    public class GenerateServiceForwarding
    {
        public static Assembly GenerateAssembly(string name, List<string> listSource, List<string> listRefAss)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Data.Entity.dll");
            cp.ReferencedAssemblies.Add("Dorado.ESB.Extensions.dll");
            cp.ReferencedAssemblies.Add("Dorado.Common.dll");
            cp.ReferencedAssemblies.AddRange(listRefAss.ToArray());

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (ass.Location != null && ass.Location != "")
                    {
                        if (!ass.Location.Contains("System.dll"))
                        {
                            if (!cp.ReferencedAssemblies.Contains(ass.Location))
                                cp.ReferencedAssemblies.Add(ass.Location);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("Dorado.ESB.GenerateServices", ex);
                }
            }

            IDictionary<string, string> version = new Dictionary<string, string>();
            if (System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().StartsWith("v4.0"))
                version.Add("CompilerVersion", "v4.0");
            else
                version.Add("CompilerVersion", "v3.5");
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(version);
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, listSource.ToArray());

            if (cr.Errors.Count > 0)
            {
                StringBuilder builder = new StringBuilder(4096);
                builder.AppendLine("Errors building '" + String.Join(",", name) + "' assembly");
                foreach (CompilerError ce in cr.Errors)
                {
                    LoggerWrapper.Logger.Error("Dorado.ESB.GenerateServices", new Exception(ce.ErrorText));
                    builder.AppendLine("  " + ce.ToString());
                    builder.AppendLine();
                }
                LoggerWrapper.Logger.Info("Dorado.ESB.GenerateServices:" + builder.ToString());
                throw new Exception("Errors building '" + String.Join(",", name) + "' assembly");
            }
            else
            {
                return cr.CompiledAssembly;
            }
        }

        private static string GetInterfaceTypeCode(ServiceModelType serviceModelType, List<string> listNamespace, List<Type[]> listTypes, bool isAuthorization, string name)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.ServiceModel.Web;");
            builder.AppendLine("using System.Reflection;");
            listNamespace.ForEach(delegate(string ns)
            {
                ns = ns.Replace("ServiceImp", "ServiceInterface");
                builder.AppendLine("using " + ns + ";");
            });
            builder.AppendLine("namespace Dorado.PlatformServices.ServiceInterface {");

            if (isAuthorization)
                builder.AppendLine(" [ServiceContract(Name = \"" + name + "\", Namespace = \"api.Dorado.com\")]");
            else
                builder.AppendLine(" [ServiceContract]");

            builder.AppendLine("public interface IWcfApi : ");
            List<string> interfaceList = new List<string>();

            listTypes.ForEach(delegate(Type[] types)
            {
                foreach (Type type in types)
                {
                    if (Regex.IsMatch(type.Namespace ?? "", @"^Dorado(\.[a-zA-Z0-9]+)*\.ServiceImp$", System.Text.RegularExpressions.RegexOptions.Singleline)
                   && Regex.IsMatch(type.Name ?? "", @"^[a-zA-Z0-9]+Provider$", System.Text.RegularExpressions.RegexOptions.Singleline))
                    {
                        interfaceList.Add("I" + type.Name);
                    }
                }
            });

            string interfaces = String.Join(",", interfaceList.ToArray());
            builder.Append(interfaces);
            builder.AppendLine("{");

            if (serviceModelType == ServiceModelType.ServiceHost)
                builder.AppendLine(PingInterfaceMethod());

            if (serviceModelType == ServiceModelType.WebServiceHost)
                builder.AppendLine(HelpInterfaceMethod());

            builder.AppendLine("}");
            builder.AppendLine("}");

            string resultBuilder = builder.ToString();
            return resultBuilder;
        }

        private static string HelpInterfaceMethod()
        {
            String help = @"

          /// <summary>
        /// 判断当前服务是否Online
        /// </summary>
        [OperationContract(Action = ""isOnline"")]
        [WebGet(UriTemplate = ""json/isOnline"", ResponseFormat = WebMessageFormat.Json)]
        void IsOnline();

        /// <summary>
        /// 获取PlatformServices提供的所有接口描述
        /// </summary>
        /// <returns></returns>
        [OperationContract(Action = ""help"")]
        [WebGet(UriTemplate = ""help"")]
        Stream GetDocumentation();

        /// <summary>
        /// 获取具体对象的类型信息
        /// </summary>
        /// <returns></returns>
        [OperationContract(Action = ""help_type"")]
        [WebGet(UriTemplate = ""/help/typeof/{typeValue}"")]
        Stream GetObjectTypeDocumentation(string typeValue);

           ";
            return help;
        }

        private static string PingInterfaceMethod()
        {
            String ping = @"

             [OperationContract(Action = ""Ping"")]
            bool Ping();

           ";
            return ping;
        }

        private static string GetServiceTypeCode(List<Type[]> listTypes, bool isAuthorization)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.ServiceModel.Web;");
            builder.AppendLine("using System.Net;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using Dorado.ESB.GenerateServices;");
            builder.AppendLine("using Dorado.ESB.Extensions;");
            builder.AppendLine("using Dorado.Common.ESB;");
            builder.AppendLine("using Dorado.PlatformServices.ServiceInterface;");
            builder.AppendLine("namespace Dorado.PlatformServices.ServiceImp {");
            builder.AppendLine("[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]");
            builder.AppendLine("public class WcfApi : ApiWcfBase,IWcfApi {");

            listTypes.ForEach(delegate(Type[] types)
            {
                foreach (Type type in types)
                {
                    if (Regex.IsMatch(type.Namespace ?? "", @"^Dorado(\.[a-zA-Z0-9]+)*\.ServiceImp$", System.Text.RegularExpressions.RegexOptions.Singleline)
                  && Regex.IsMatch(type.Name ?? "", @"^[a-zA-Z0-9]+Provider$", System.Text.RegularExpressions.RegexOptions.Singleline))
                    {
                        MethodInfo[] methods = GetMethods(type);
                        BuildMethod(builder, methods, type, isAuthorization);
                    }
                }
            });

            builder.Append(PingMethod());

            builder.Append(HelpMethod());

            builder.AppendLine("}");
            builder.AppendLine(HelpClass());
            builder.AppendLine("}");

            string resultBuilder = builder.ToString();
            return resultBuilder;
        }

        public static string HelpMethod()
        {
            string helpImp = string.Empty;
            using (System.IO.Stream stream =
            System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "Dorado.ESB.GenerateServices.Resources.HelpImpCS.txt"))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    helpImp = reader.ReadToEnd();

                    reader.Close();
                }
                stream.Close();
            }
            return helpImp;
        }

        private static string PingMethod()
        {
            String ping = @"
            public bool Ping()
            {
                try
                {
                    return true;
                 }
                catch (Exception ex)
                 {
                    ApiWcfBase.LogException(ex, ""Dorado.ESB.GenerateServices"");
                    return false;
                }
             }";
            return ping;
        }

        private static void BuildMethod(StringBuilder builder, MethodInfo[] methods, Type type, bool isAuthorization)
        {
            foreach (MethodInfo method in methods)
            {
                //modify 2010/10/21
                //if (isAuthorization)
                //    builder.Append("[Dorado.ESB.Extensions.Auth.Authorization(true)]");
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
                }
                builder.AppendLine(")");
                builder.AppendLine("{");

                builder.AppendLine("try{");
                if (method.ReturnType != typeof(void))
                    builder.Append("return ");
                builder.Append(type.FullName + ".Instance." + method.Name + "(");
                for (int i = 0; i < prms.Length; i++)
                {
                    ParameterInfo prm = prms[i];
                    if (i != 0)
                        builder.Append(",");
                    if (prm.IsOut)
                        builder.Append("out " + prm.Name);
                    else
                        builder.Append(prm.Name);
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
                else if (method.ReturnType == typeof(DateTime))
                {
                    returnValue = "return new DateTime();";
                }
                else if (method.ReturnType.IsEnum)
                {
                    string enumTypeName = GetTypeName(method.ReturnType);
                    returnValue = "return (" + enumTypeName + ")Enum.Parse(typeof(" + enumTypeName + "),\"1\");";
                }
                else
                {
                    returnValue = "return null;";
                }

                builder.AppendLine(@"}
                    catch (FaultException<CustomFault> ex)
                    {
                         //+++Custom ""Dorado.ESB.CustomFault"" exception thrown to the client directly+++
                         ApiWcfBase.LogException(ex, ""Dorado.ESB.GenerateServices"");
                         throw new System.ServiceModel.FaultException<CustomFault>(ex.Detail, ex.Message);
                    }
                    catch (Exception ex)
                    {
                         ApiWcfBase.LogException(ex, ""Dorado.ESB.GenerateServices"");
                    ");

                //+++++When an error, returns a custom error value+++++
                // builder.AppendLine(returnValue);

                //++++++Unknown exception thrown to the client directly++++++++
                builder.AppendLine("throw ex;");

                builder.AppendLine("}");
                builder.AppendLine("}");
            }
        }

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

        private static string HelpClass()
        {
            String helpClass = string.Empty;

            helpClass = @"

                    internal class MethodDoc
                    {
                        public string MethodName;
                        public string ServiceName;
                        public string RequestType;
                        public string RequestUrl;
                        public string RequestParams;
                        public string ReturnTypeName;
                        public string desc;
                    }

                    internal class ServiceDoc
                    {
                        public string Name;
                        public string Desc;
                        public List<MethodDoc> MethodDocList = new List<MethodDoc>();
                    }";
            return helpClass;
        }

        private static MethodInfo[] GetMethods(Type type)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            List<MethodInfo> ret = new List<MethodInfo>();
            foreach (MethodInfo info in infos)
            {
                object[] attrs = info.GetCustomAttributes(typeof(OperationContractAttribute), true);
                if (attrs.Length != 0)
                    ret.Add(info);
            }
            int count = ret.Count();
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

        public static Assembly GetGenerateServiceAssemblyForwarding(Forwarding forwarding, ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes, bool isAuthorization, string name)
        {
            List<string> wcfNameSpaces = serviceNamespaces.Split(';').ToList();
            List<string> listSource = new List<string>();
            List<string> listRefAss = new List<string>();
            string code = string.Empty;
            if (forwarding == Forwarding.Interface)
            {
                code = GetInterfaceTypeCode(serviceModelType, wcfNameSpaces, listTypes, isAuthorization, name);
                listSource.Add(code);
            }
            else if (forwarding == Forwarding.Implement)
            {
                code = GetServiceTypeCode(listTypes, isAuthorization);
                listSource.Add(code);
            }
            else if (forwarding == Forwarding.All)
            {
                code = GetInterfaceTypeCode(serviceModelType, wcfNameSpaces, listTypes, isAuthorization, name);
                listSource.Add(code);
                code = GetServiceTypeCode(listTypes, isAuthorization);
                listSource.Add(code);
            }

            Assembly assembly = GenerateAssembly(name, listSource, listRefAss);

            return assembly;
        }
    }
}