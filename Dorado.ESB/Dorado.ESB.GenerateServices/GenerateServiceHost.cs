using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace Dorado.ESB.GenerateServices
{
    public class GenerateServiceHost
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

        private static string GetServiceTypeCode(List<Type[]> listTypes)
        {
            //2010/05/18
            MethodInfo[] methods = GetMethods(null);

            //MethodInfo[] methods = GetMethods(listTypes);
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using Dorado.ESB.GenerateServices;");
            builder.AppendLine("namespace " + WcfNameSpace + "{");

            builder.AppendLine("[ServiceContract(Name = \"DoradoPlatformServiceWcf\", Namespace = \"api.cnmsprod.local\")]");

            builder.AppendLine("public class WcfApi : ApiWcfBase,IWcfApi {");
            foreach (MethodInfo method in methods)
            {
                object[] attrs = method.GetCustomAttributes(typeof(OperationContractAttribute), true);

                OperationContractAttribute attr = attrs[0] as OperationContractAttribute;

                string operContract = "[OperationContract(Action = \"" + attr.Action + "\", ";

                if (attr.IsOneWay)
                    operContract += "IsOneWay = true";
                else
                    operContract += "IsOneWay = false";

                builder.AppendLine(operContract + ") ]");

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

                //----------------------------------------------------------
                //Del By jiangsong 2009-11-14
                //builder.AppendLine("using (new ContextScope()){");
                //----------------------------------------------------------
                builder.AppendLine("try{");
                if (method.ReturnType != typeof(void))

                    //builder.Append("return ");

                    //----------------------------------------------------------
                    //Add By jiangsong 2009-11-16
                    //builder.Append("return \"Dorado.com\";");
                    //----------------------------------------------------------

                    //---------------------begin------2010-02-24-14:13-------------------------------
                    //builder.Append("Assembly assembly = AppDomain.CurrentDomain.Load(\"PlatformServiceComponent.HelloWorld\", AppDomain.CurrentDomain.Evidence);");
                    //builder.Append("PlatformServiceComponent.HelloWorld.IHelloWorldService service = (PlatformServiceComponent.HelloWorld.IHelloWorldService)assembly.CreateInstance(\"PlatformServiceComponent.HelloWorld.HelloWorldService\");");
                    //builder.Append("return service.GetHelloWorld() + DateTime.Now.ToString() + \" \" + DateTime.Now.Millisecond.ToString();");
                    //----------------------end------------------------------------

                    //------------------------begin----------------------------------
                    //builder.Append("new " + serviceType.FullName + "()." + method.Name + "(");
                    //for (int i = 0; i < prms.Length; i++)
                    //{
                    //    ParameterInfo prm = prms[i];
                    //    if (i != 0)
                    //        builder.Append(",");
                    //    if (prm.IsOut)
                    //        builder.Append("out " + prm.Name);
                    //    else
                    //        builder.Append(prm.Name);
                    //}
                    //builder.AppendLine(");");

                    //string returnValue = "";
                    //if (method.ReturnType == typeof(void))
                    //{
                    //    returnValue = "";

                    //}
                    //else if (method.ReturnType == typeof(int) || method.ReturnType == typeof(long))
                    //{
                    //    returnValue = "return -1;";
                    //}
                    //else if (method.ReturnType == typeof(bool))
                    //{
                    //    returnValue = "return false;";
                    //}
                    //else
                    //{
                    //    returnValue = "return null;";
                    //}
                    //---------------------end-------------------------------------

                    builder.AppendLine(@"}
                    catch (Exception ex)
                    {
                         ApiWcfBase.LogException(ex, ""Dorado.ESB.Server"");
                    ");

                //----------------------------------------------------------
                //Add By jiangsong 2009-11-16
                string returnValue = "";
                returnValue = "return null;";
                //----------------------------------------------------------

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

            return builder.ToString();
        }

        private static Assembly GetWcfServiceTypeAssembly(List<Type[]> listTypes)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.CompilerOptions = "/optimize";
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(Dorado.ESB.GenerateServices.ApiWcfBase).Assembly.Location);
            cp.ReferencedAssemblies.Add(typeof(LoggerWrapper).Assembly.Location);
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

            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            string code = GetServiceTypeCode(listTypes);

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

        private static Type GenerateWcfServiceType(List<Type[]> listTypes)
        {
            //2010/05/18
            Assembly ass = GetWcfServiceTypeAssembly(listTypes);
            return ass.GetType("Dorado.PlatformServices.ServiceImp.WcfApi");
        }

        private static Type wcfServiceType;

        public static Type GetWcfServiceType(List<Type[]> listTypes, string serviceNamespaces)
        {
            if (wcfServiceType == null)
            {
                wcfServiceType = GenerateWcfServiceType(listTypes);
            }
            return wcfServiceType;
        }
    }
}