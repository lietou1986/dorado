using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public class JsonClientGenerator
    {
        private const string DefaultNamespace = "__apiJson";

        private static string GetRequestClassName(MethodInfo info)
        {
            return "__" + info.Name + "Request";
        }

        private static string GetProxyClassName(Type type)
        {
            return "__" + type.FullName.Replace('.', '_') + "_Proxy";
        }

        private static string GetProxyFactoryClassName(Type type)
        {
            return "__" + type.FullName.Replace('.', '_') + "_FactoryProxy";
        }

        private static string GenerateRequestClass(MethodInfo info)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("[DataContract]");
            builder.AppendLine("public class " + GetRequestClassName(info) + "{");
            ParameterInfo[] prms = info.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo prm = prms[i];
                builder.AppendLine("[DataMember]");

                string typeName = AssemblyGeneratorHelper.GetTypeName(prm.ParameterType);
                builder.AppendLine("public " + typeName + " " + prm.Name + ";");
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static JsonAttribute GetJsonAttribute(MethodInfo info)
        {
            object[] attrs = info.GetCustomAttributes(typeof(JsonAttribute), true);
            if (attrs.Length == 0) return null;
            return attrs[0] as JsonAttribute;
        }

        private static string GenerateMethod(MethodInfo info)
        {
            StringBuilder builder = new StringBuilder(8192);
            ParameterInfo[] prms = info.GetParameters();
            JsonAttribute attr = GetJsonAttribute(info);
            if (attr == null)
            {
                builder.AppendLine(AssemblyGeneratorHelper.GenerateMethodDeclaration(info));
                builder.AppendLine("{");
                builder.AppendLine("throw new NotImplementedException (\"Method '" + info.Name + "' is not implemented\");");
                builder.AppendLine("}");
                return builder.ToString();
            }
            if (attr.Method == JsonMethod.Post && prms.Length > 0)
            {
                builder.AppendLine(GenerateRequestClass(info));
            }

            builder.AppendLine(AssemblyGeneratorHelper.GenerateMethodDeclaration(info));
            builder.AppendLine("{");
            if (attr.Method == JsonMethod.Post)
            {
                builder.AppendLine("WebRequest __request = GetWebRequest(new Uri(Url+\"" + attr.UriTemplate + "\"));");
                builder.AppendLine("__request.Method = \"POST\";");
                builder.AppendLine("__request.ContentType  = \"application/json\";");

                string reqClassName = GetRequestClassName(info);

                #region Handler request

                if (prms.Length > 0)
                {
                    builder.AppendLine("using (Stream __stream = __request.GetRequestStream()){");
                    builder.AppendLine(reqClassName + " __req = new " + reqClassName + "();");

                    for (int i = 0; i < prms.Length; i++)
                    {
                        ParameterInfo prm = prms[i];
                        builder.Append("__req." + prm.Name + "=" + prm.Name + ";");
                    }
                    builder.AppendLine("DataContractJsonSerializer __ser = new DataContractJsonSerializer (typeof(" + reqClassName + "));");
                    builder.AppendLine("__ser.WriteObject(__stream,__req);");
                    builder.AppendLine("}");
                }

                #endregion Handler request
            }
            else
            {
                string strFormat = attr.UriTemplate;
                for (int i = 0; i < prms.Length; i++)
                {
                    ParameterInfo prm = prms[i];
                    strFormat = strFormat.Replace("{" + prm.Name + "}", "{" + i + "}");
                }
                builder.Append("string url = string.Format(Url+\"" + strFormat + "\"");
                if (prms.Length > 0)
                {
                    builder.Append(",");
                    builder.Append(AssemblyGeneratorHelper.GetMethodParameterString(info));
                }

                builder.AppendLine(");");

                builder.AppendLine("WebRequest __request = GetWebRequest(new Uri(url));");
                builder.AppendLine("__request.Method = \"GET\";");
            }

            #region Handler response

            builder.AppendLine("using (HttpWebResponse __response = GetWebResponse(__request) as HttpWebResponse)");
            builder.AppendLine("{");
            builder.AppendLine("if(__response.StatusCode != HttpStatusCode.OK) throw new JsonWebException(__response);");
            if (info.ReturnType != typeof(void))
            {
                string retTypeName = AssemblyGeneratorHelper.GetTypeName(info.ReturnType);
                builder.AppendLine("using (Stream __stream = __response.GetResponseStream()){");
                builder.AppendLine("DataContractJsonSerializer __ser = new DataContractJsonSerializer (typeof(" + retTypeName + "));");
                builder.AppendLine("return (" + retTypeName + ")__ser.ReadObject(__stream);");
                builder.AppendLine("}");
            }
            builder.AppendLine("}");

            #endregion Handler response

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string GenerateProxyClass(Type type)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("public class " + GetProxyClassName(type) + " : HttpWebClientProtocol ," + type.FullName + "{");
            MethodInfo[] methods = AssemblyGeneratorHelper.GetMethods(type, null);
            foreach (MethodInfo method in methods)
            {
                builder.AppendLine(GenerateMethod(method));
            }
            builder.AppendLine("}");
            return builder.ToString();
        }

        private static string GenerateProxyFactoryClass(Type type)
        {
            StringBuilder builder = new StringBuilder(4096);
            builder.AppendLine("public class " + GetProxyFactoryClassName(type) + " : IJsonClientFactory<" + type.FullName + ">{");
            builder.AppendLine("public " + type.FullName + " GetJsonProtocolObject( string baseUrl){");
            string proxyClassName = GetProxyClassName(type);
            builder.AppendLine(proxyClassName + " proxy =  new " + proxyClassName + "(); ");
            builder.AppendLine(" proxy.UseDefaultCredentials= false; proxy.Url = baseUrl; return proxy;");
            builder.AppendLine("}");
            builder.AppendLine("}");
            return builder.ToString();
        }

        public static Assembly GenerateProxyAssembly(string assName, Type[] types)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Net;");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using System.Web.Services.Protocols;");
            builder.AppendLine("using System.Runtime.Serialization;");
            builder.AppendLine("using System.Runtime.Serialization.Json;");
            builder.AppendLine("using Dorado.ESB.ClientProxyFactory.Proxy;");
            builder.AppendLine("using Dorado.Configuration;");

            builder.AppendLine("namespace " + DefaultNamespace + "{");
            foreach (Type type in types)
            {
                builder.AppendLine(GenerateProxyFactoryClass(type));
                builder.AppendLine(GenerateProxyClass(type));
            }

            builder.AppendLine("}");

            List<string> refAssLocations = new List<string>();
            refAssLocations.Add(typeof(System.Web.Services.Protocols.HttpWebClientProtocol).Assembly.Location);
            refAssLocations.Add(typeof(JsonAttribute).Assembly.Location);
            refAssLocations.Add(typeof(WcfClientGenerator).Assembly.Location);
            refAssLocations.Add(typeof(DataContractAttribute).Assembly.Location);
            refAssLocations.Add(typeof(DataContractSerializer).Assembly.Location);
            refAssLocations.Add(typeof(System.Runtime.Serialization.Json.DataContractJsonSerializer).Assembly.Location);
            refAssLocations.Add(typeof(System.Xml.XmlReader).Assembly.Location);
            refAssLocations.Add(typeof(LoggerWrapper).Assembly.Location);
            foreach (Type type in types)
            {
                if (!refAssLocations.Contains(type.Assembly.Location))
                    refAssLocations.Add(type.Assembly.Location);
            }

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass is System.Reflection.Emit.AssemblyBuilder)
                    continue;

                try
                {
                    if (ass.Location != null && ass.Location != "")
                    {
                        if (!ass.Location.Contains("System.dll"))
                        {
                            if (!refAssLocations.Contains(ass.Location))
                                refAssLocations.Add(ass.Location);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LoggerWrapper.Logger.Error(ex, "JsonClientGenerator");
                    LoggerWrapper.Logger.Info("JsonClientGenerator " + ex.Message);
                }
            }

            string code = builder.ToString();

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            /*
            if (!Dorado.Configuration.MaintenanceConfig.FunctionIsDisabled(1, "JsonClientProxyOutput"))
            {
                try
                {
                    System.IO.File.WriteAllText(@"c:\" + assName + ".cs", code);
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error(ex, "JsonClientGenerator");
                }
            }
            */

            //----------------------------------------

            return AssemblyGeneratorHelper.GenerateAssembly(assName,
                new string[] { code },
                refAssLocations.ToArray());
        }

        public static object[] GenerateProxyFactories(string assName, Type[] types)
        {
            Assembly ass = GenerateProxyAssembly(assName, types);
            if (ass == null) return null;
            else
            {
                object[] factories = new object[types.Length];
                for (int i = 0; i < types.Length; i++)
                    factories[i] = ass.CreateInstance(DefaultNamespace + "." + GetProxyFactoryClassName(types[i]));
                return factories;
            }
        }
    }

    public class JsonWebException : Exception
    {
        public HttpStatusCode StatusCode;
        public string StatusDescription;

        private static string GetResponseString(HttpWebResponse rsp)
        {
            using (StreamReader reader = new StreamReader(rsp.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public JsonWebException(HttpWebResponse rsp)
            : base(GetResponseString(rsp))
        {
            StatusCode = rsp.StatusCode;
            StatusDescription = rsp.StatusDescription;
        }
    }
}