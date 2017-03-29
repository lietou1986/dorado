using Dorado.ESB.ClientProxyFactory.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    internal class WcfClientGenerator
    {
        private const string NameSpace = "__wcfProxy";

        private WcfClientGenerator()
        {
        }

        private static void GeneratePing_Source(StringBuilder builder, string impClsName)
        {
            builder.AppendLine(@"
            public System.Boolean Ping()
            {
                    bool IsPingSucceed = false;
                    EndpointAddress endpointAddress = new StrategyContext(""random"").GetResult(wcfClientConfig.ServiceStatus, String.Empty);
                    " + impClsName + @" proxy = GetProxyFromPool(endpointAddress);
                    try
                    {
                        //LoggerWrapper.Logger.Info(String.Format(""heartbeating,address:{0},tag:{1}"",endpointAddress.ToString(),heartbeatTag));
                        IsPingSucceed = proxy.Ping();
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                        proxy = null;
                        LoggerWrapper.Logger.Error(ex, ""Dorado.ESB.ClientProxyFactory"");
                    }
                    finally
                    {
                        if (poolList.Count>0 && proxy != null)
                            poolList[endpointAddress.ToString()].CheckInPool(proxy);
                    }
                    return IsPingSucceed;
            }
            ");
        }

        private static void PingAllServerRefreshLocalServiceStatus_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
            private void PingAllServerRefreshLocalServiceStatus()
            {
               bool IsPingSucceed = false;
               wcfClientConfig.ABC.RemoteAddress.ForEach(
                delegate(EndpointAddress endpointAddress)
                    {
                        try
                        {
                            minDelayStopwatch = new Stopwatch();
                            minDelayStopwatch.Start();
                            IsPingSucceed = Ping();
                            minDelayStopwatch.Stop();
                            RefreshLocalServiceStatus(endpointAddress.ToString(), IsPingSucceed, minDelayStopwatch.Elapsed.TotalMilliseconds);
                        }
                        catch
                        {
                            LoggerWrapper.Logger.Info(String.Format(""ping can not connect to remote server,address:{0}"",endpointAddress.ToString()));
                            RefreshLocalServiceStatus(endpointAddress.ToString(), IsPingSucceed, 0.0);
                        }
                    }
                );
             }");
        }

        private static void RefreshLocalServiceStatus_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
               private void RefreshLocalServiceStatus(string address, bool pulsate, double delayTime)
                 {
                     wcfClientConfig.ServiceStatus.ListHeartbeatStatus.ForEach(
                    delegate(HeartbeatStatus heartbeatStatus)
                    {
                        if (String.Compare(heartbeatStatus.Address, address, StringComparison.CurrentCultureIgnoreCase)==0)
                        {
                            heartbeatStatus.Pulsate = pulsate;
                            heartbeatStatus.Delay = delayTime;
                        }
                    }
                );
               }
            ");
        }

        private static void TimerHeartbeat_Elapsed_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
             private void TimerHeartbeat_Elapsed(object sender, EventArgs e)
             {
                 heartbeatThread = new Thread(new ThreadStart(PingAllServerRefreshLocalServiceStatus));
                 heartbeatThread.Start();
             }
            ");
        }

        private static void Destructors_Source(StringBuilder builder, string impFactoryClsName)
        {
            string destructorsString = @"
             ~[impFactoryClsName]()
            {
                dictionaryCache.Clear();
                dictionaryCache = null;
                poolList.Clear();
                poolList = null;
            }";
            builder.AppendLine(destructorsString.Replace("[impFactoryClsName]", impFactoryClsName));
        }

        private static void InitTimerHeartbeat_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
                 private void InitTimerHeartbeat()
                 {
                    timerHeartbeat = new System.Timers.Timer();
                    timerHeartbeat.Interval = wcfClientConfig.Heartbeat.Interval;
                    timerHeartbeat.Elapsed += TimerHeartbeat_Elapsed;
                    timerHeartbeat.Enabled = true;
                }
            ");
        }

        private static void CreateHeartbeat_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
              private void CreateHeartbeat()
               {
                if (wcfClientConfig.Heartbeat.Enabled)
                {
                    LoggerWrapper.Logger.Info(""----------Heartbeat Enabled Run----------"");
                    InitTimerHeartbeat();
                }
             }
            ");
        }

        private static void CreateDictionaryCache_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
            private void CreateDictionaryCache()
            {
                if (String.Compare(wcfClientConfig.ControlLevel, ""Single"", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    InitDictionaryCache();
                    LoggerWrapper.Logger.Info(""----------Init DictionaryCache----------"");
                }
            }
            ");
        }

        private static void CreateChannelPool_Source(StringBuilder builder, string impFactoryClsName, string intName)
        {
            string createChannelPoolString = @"
            private void CreateChannelPool() {
            LoggerWrapper.Logger.Info(""-----------Create Channel Pool----------"");
            if (this.wcfClientConfig.ChannelPool.Enabled)
            {
                this.wcfClientConfig.ABC.RemoteAddress.ForEach(delegate(EndpointAddress endpointAddress)
                {
                    poolList.Add(
                        new KeyValuePair<string, WcfProxyPool<[intName]>>
                            (
                             endpointAddress.ToString(),
                             new Dorado.ESB.ClientProxyFactory.Proxy.WcfProxyPool<[intName]>(this, this.wcfClientConfig.ChannelPool))
                            );
                });

                LoggerWrapper.Logger.Info(""Initialize [impFactoryClsName], serviceName:"" + wcfClientConfig.Name + "",PoolEnabled:"" + wcfClientConfig.ChannelPool.Enabled);
             }}";
            builder.AppendLine(createChannelPoolString.Replace("[impFactoryClsName]", impFactoryClsName).Replace("[intName]", intName));
        }

        private static void Constructor_Source(StringBuilder builder, string impFactoryClsName)
        {
            string constructorString = @"
            public [impFactoryClsName]()
            {
            }
            public [impFactoryClsName](Dorado.ESB.ClientProxyFactory.Config.WcfClientConfig wcfClientConfig)
            {
                this.wcfClientConfig = wcfClientConfig;
                CreateChannelPool();
                CreateDictionaryCache();
                CreateHeartbeat();
             }";
            builder.AppendLine(constructorString.Replace("[impFactoryClsName]", impFactoryClsName));
        }

        private static void GetProxyFromPool_Source(StringBuilder builder, string impClsName)
        {
            string proxyFromPoolString = @"
             private [impClsName] GetProxyFromPool(EndpointAddress endpointAddress)
             {
                    [impClsName] proxy = null;
                    try
                    {
                         if (poolList.Count>0)
                         {
                                if(poolList[endpointAddress.ToString()] != null)
                                 {
                                   proxy = poolList[endpointAddress.ToString()].GetProxy() as [impClsName];
                                   LoggerWrapper.Logger.Info(endpointAddress.ToString()+"" proxy from pool"");
                                 }
                          }
                        else
                         {
                              proxy = new [impClsName](wcfClientConfig.ABC.Binding, endpointAddress);
                              LoggerWrapper.Logger.Info(endpointAddress.ToString()+"" proxy new"");
                          }
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error(ex, ""Dorado.ESB.ClientProxyFactory"");
                        throw ex;
                    }
                    return proxy;
             }";
            builder.AppendLine(proxyFromPoolString.Replace("[impClsName]", impClsName));
        }

        private static void InitDictionaryCache_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
            private void InitDictionaryCache()
            {
                AssembliesProvider.Instance.CurrentAssemblies.Where(
                c => Regex.IsMatch(c.FullName.Split(',')[0] ?? """", @""^Dorado(\.[a-zA-Z0-9]+)*\.ServiceImp$"", System.Text.RegularExpressions.RegexOptions.Singleline) == true).ToList().
                ForEach(delegate(Assembly assembly)
                {
                    List<Type> listType = assembly.GetTypes().Where(c => Regex.IsMatch(c.Name ?? """", @""^[a-zA-Z0-9]+Provider$"", System.Text.RegularExpressions.RegexOptions.Singleline) == true).ToList();
                    listType.ForEach(delegate(Type type)
                    {
                        string key = String.Format(""{0}.{1}"", type.Namespace, type.Name);
                        if (!dictionaryCache.ContainsKey(key))
                        {
                            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty;
                            Type objType = (Type)type.InvokeMember(""Instance"", bindingFlags, null, null, null).GetType();
                            dictionaryCache.Add(key, objType);
                            LoggerWrapper.Logger.Info(""----------Load DictionaryCache："" + String.Format(""{0}.{1}"", type.Namespace, type.Name) + ""---------"");
                        }
                    }
                    );
                }
                );
             }
            ");
        }

        private static void GetEndpointAddressFromStrategyContext_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
            private EndpointAddress GetEndpointAddressFromStrategyContext(string address)
            {
                return new StrategyContext(wcfClientConfig.NodeStrategy.LoadBalanceStrategy).GetResult(wcfClientConfig.ServiceStatus, address);
            }
            ");
        }

        private static void StopServices_Source(StringBuilder builder)
        {
            builder.AppendLine(@"
                   public static void StopServices()
                   {
                        LoggerWrapper.Logger.Info(""+++Stop Services Heartbeat,Reset new Services+++"");
                        timerHeartbeat.Enabled = false;
                 }
            ");
        }

        private static void GenerateWcfMethodSource_Source_Local(StringBuilder builder, string impClsName, string intName, MethodInfo method)
        {
            builder.AppendLine("{");
            builder.AppendLine("Type objType = null;");
            builder.AppendLine(String.Format(@"string typeName = ""{0}"";", method.DeclaringType.ToString().Replace("ServiceInterface", "ServiceImp").Replace(method.DeclaringType.Name, method.DeclaringType.Name.Substring(1))));
            builder.AppendLine("dictionaryCache.TryGetValue(typeName, out objType);");
            builder.AppendLine("object target = objType.InvokeMember(null, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);");
            builder.AppendLine();
            if (method.ReturnType != typeof(void))
                builder.Append("return (" + AssemblyGeneratorHelper.GetTypeName(method.ReturnType) + ")");
            builder.Append("objType.InvokeMember(\"" + method.Name + "\", BindingFlags.InvokeMethod, null, target, new object[] { ");
            builder.Append(AssemblyGeneratorHelper.GetMethodParameterString(method));
            builder.AppendLine("});");
            builder.AppendLine("}");
        }

        private static void GenerateWcfMethodSource_Source_None(StringBuilder builder, string impClsName, string intName, MethodInfo method)
        {
            builder.AppendLine("{");
            builder.AppendLine("EndpointAddress endpointAddress = GetEndpointAddressFromStrategyContext(String.Empty);");
            builder.AppendLine(impClsName + " proxy = GetProxyFromPool(endpointAddress);");
            StringBuilder tryBuilder = new StringBuilder();
            tryBuilder.AppendLine("try{");
            if (method.ReturnType != typeof(void))
                tryBuilder.Append("return ");
            tryBuilder.Append("proxy." + method.Name + "(");
            tryBuilder.Append(AssemblyGeneratorHelper.GetMethodParameterString(method));
            tryBuilder.AppendLine(");}");
            builder.AppendLine(tryBuilder.ToString());
            builder.AppendLine(@"
            catch (Exception ex)
            {
                    proxy.Abort();
                    proxy = null;
                    LoggerWrapper.Logger.Error(ex, ""Dorado.ESB.ClientProxyFactory"");
                    throw ex;
            }
            finally
            {
                    if (poolList.Count>0 && proxy != null){
                    poolList[endpointAddress.ToString()].CheckInPool(proxy);
                    }
                }");
            builder.AppendLine("}");
            tryBuilder = null;
        }

        private static void GenerateWcfMethodSource_Source_Switch(StringBuilder builder, string impClsName, string intName, MethodInfo method)
        {
            builder.AppendLine("{");
            builder.AppendLine("EndpointAddress endpointAddress = GetEndpointAddressFromStrategyContext(String.Empty);");
            builder.AppendLine(impClsName + " proxy = GetProxyFromPool(endpointAddress);");
            StringBuilder tryBuilder = new StringBuilder();
            tryBuilder.AppendLine("try{");
            if (method.ReturnType != typeof(void))
                tryBuilder.Append("return ");
            tryBuilder.Append("proxy." + method.Name + "(");
            tryBuilder.Append(AssemblyGeneratorHelper.GetMethodParameterString(method));
            tryBuilder.AppendLine(");}");
            builder.AppendLine(tryBuilder.ToString());
            string catchString = @"
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error(ex, ""Dorado.ESB.ClientProxyFactory--Try Reconnect"");
                        proxy.Abort();
                        proxy = null;
                        LoggerWrapper.Logger.Info(String.Format(""----------Call [methodName] Failed,Try Reconnect, Strategy is {0}----------"", wcfClientConfig.NodeStrategy.FailedStrategy));
                         endpointAddress = GetEndpointAddressFromStrategyContext(endpointAddress.ToString());
                         proxy = GetProxyFromPool(endpointAddress);
                    ";
            catchString = catchString.Replace("[methodName]", method.Name);
            builder.AppendLine(catchString);
            builder.AppendLine(tryBuilder.ToString());
            builder.AppendLine(@"
                    catch (Exception rex)
                    {
                        proxy.Abort();
                        proxy = null;
                        LoggerWrapper.Logger.Error(rex, ""Dorado.ESB.ClientProxyFactory"");
                        throw rex;
                   } ");
            builder.AppendLine("}");
            builder.AppendLine(@"
                 finally
                {
                     if (poolList.Count>0 && proxy != null)
                             poolList[endpointAddress.ToString()].CheckInPool(proxy);
                }");
            builder.AppendLine("}");
            tryBuilder = null;
        }

        private static void GenerateWcfMethodSource_Source_Try(StringBuilder builder, string impClsName, string intName, MethodInfo method)
        {
            builder.AppendLine("{");
            builder.AppendLine("EndpointAddress endpointAddress = GetEndpointAddressFromStrategyContext(String.Empty);");
            builder.AppendLine(impClsName + " proxy = GetProxyFromPool(endpointAddress);");
            StringBuilder tryBuilder = new StringBuilder();
            tryBuilder.AppendLine("try{");
            if (method.ReturnType != typeof(void))
                tryBuilder.Append("return ");
            tryBuilder.Append("proxy." + method.Name + "(");
            tryBuilder.Append(AssemblyGeneratorHelper.GetMethodParameterString(method));
            tryBuilder.Append(");}");
            builder.AppendLine(tryBuilder.ToString());
            string catchString = @"
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error(ex, ""Dorado.ESB.ClientProxyFactory--Try Reconnect"");
                        proxy.Abort();
                        proxy = null;
                        LoggerWrapper.Logger.Info(String.Format(""----------Call [methodName] Failed,Try Reconnect, Strategy is {0}----------"", wcfClientConfig.NodeStrategy.FailedStrategy));
                    ";
            catchString = catchString.Replace("[methodName]", method.Name);
            builder.AppendLine(catchString);

            if (method.ReturnType != typeof(void))
                builder.AppendLine("object returnValue = null;");

            builder.AppendLine("for (int i = 0; i < wcfClientConfig.NodeStrategy.TryNumber; i++) {");
            builder.AppendLine(@"LoggerWrapper.Logger.Info(String.Format(@""----------Try Reconnect Call [methodName] , calling, {0} th----------"",i));".Replace("[methodName]", method.Name));
            builder.AppendLine("proxy = GetProxyFromPool(endpointAddress);");

            builder.AppendLine(tryBuilder.ToString().Insert(tryBuilder.ToString().Length - 1, "break;").Replace("return", "returnValue = "));

            builder.AppendLine(@"
                      catch (Exception rex)
                      {
                        proxy.Abort();
                        proxy = null;
                        if (i == wcfClientConfig.NodeStrategy.TryNumber - 1)
                        {
                            LoggerWrapper.Logger.Error(rex, ""Dorado.ESB.ClientProxyFactory"");
                            throw rex;
                        }
                   } ");

            builder.AppendLine("}");
            if (method.ReturnType != typeof(void))
                builder.AppendLine("return (" + AssemblyGeneratorHelper.GetTypeName(method.ReturnType) + ")returnValue;");

            builder.AppendLine("}");

            builder.AppendLine(@"
                 finally
                {
                     if (poolList.Count>0 && proxy != null)
                             poolList[endpointAddress.ToString()].CheckInPool(proxy);
                }");
            builder.AppendLine("}");
            tryBuilder = null;
        }

        private static string PingInterfaceMethod_Source()
        {
            String ping = @"

             [OperationContract(Action = ""Ping"")]
            bool Ping();

           ";
            return ping;
        }

        private static Type GetProviderType(Type type, bool isWcfapi)
        {
            if (isWcfapi)
            {
                if (Regex.IsMatch(type.Namespace, @"^Dorado(\.[a-zA-Z0-9]+)*\.ServiceInterface$", System.Text.RegularExpressions.RegexOptions.Singleline)
                && type.Name.ToLower() == "iwcfapi" && type.IsInterface)
                    return type;
                else
                    return null;
            }
            else
            {
                if (Regex.IsMatch(type.Namespace, @"^Dorado(\.[a-zA-Z0-9]+)*\.ServiceInterface$", System.Text.RegularExpressions.RegexOptions.Singleline)
            && Regex.IsMatch(type.Name, @"^[a-zA-Z0-9]+Provider$", System.Text.RegularExpressions.RegexOptions.Singleline) && type.IsInterface)
                    return type;
                else
                    return null;
            }
        }

        public static List<MethodInfo[]> GetListMethod(Type[] serviceTypes, bool isWcfapi)
        {
            List<MethodInfo[]> listMethodInfo = new List<MethodInfo[]>();
            foreach (Type type in serviceTypes)
            {
                if (GetProviderType(type, isWcfapi) != null)
                {
                    listMethodInfo.Add(AssemblyGeneratorHelper.GetMethods(type, typeof(OperationContractAttribute)));
                }
            }

            return listMethodInfo;
        }

        public static string GenerateInterfaceTypeSource(Type[] serviceTypes, string ns)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("namespace " + ns + "{");
            builder.AppendLine("[ServiceContract]");
            builder.AppendLine("public interface IWcfApi : ");
            List<string> interfaceList = new List<string>();
            foreach (Type type in serviceTypes)
            {
                if (GetProviderType(type, false) != null)
                {
                    interfaceList.Add(type.Name);
                }
            }
            string interfaces = String.Join(",", interfaceList.ToArray());
            builder.Append(interfaces);
            builder.AppendLine("{");
            builder.AppendLine(PingInterfaceMethod_Source());
            builder.AppendLine("}");
            builder.AppendLine("}");

            string resultBuilder = builder.ToString();
            return resultBuilder;
        }

        public static string GenerateWcfClientSource(List<MethodInfo[]> listMehtodInfo, string impClsName, string intName, bool wrapper)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.ServiceModel.Description;");
            builder.AppendLine("namespace " + NameSpace + "{");

            builder.AppendLine("public class " + impClsName + " : System.ServiceModel.ClientBase<" + intName + ">, " + intName + "{");
            builder.AppendLine("public " + impClsName + "(){}");
            builder.AppendLine("public " + impClsName + "(string endpointConfigurationName) :base(endpointConfigurationName){}");
            builder.AppendLine("public " + impClsName + "(string endpointConfigurationName, string remoteAddress):base(endpointConfigurationName,remoteAddress){}");
            builder.AppendLine("public " + impClsName + "(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :base(endpointConfigurationName, remoteAddress){}");

            builder.AppendLine("public " + impClsName +
                @"(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :base(binding, remoteAddress)
                    {
                        if (binding!=null)
                        {
                                if (binding is  System.ServiceModel.WebHttpBinding)
                                    this.Endpoint.Behaviors.Add(new System.ServiceModel.Description.WebHttpBehavior());
                                this.Endpoint.Behaviors.Add(new Dorado.ESB.ClientProxyFactory.Behaviors.EndpointBehaviors.DoradoClientEndpointBehavior());
                        }
                    foreach (OperationDescription op in this.Endpoint.Contract.Operations)
                    {
                        //DataContractSerializerOperationBehavior dataContractBehavior =  op.Behaviors.Find<DataContractSerializerOperationBehavior>()  as DataContractSerializerOperationBehavior;
                        DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = op.Behaviors[typeof(DataContractSerializerOperationBehavior)] as DataContractSerializerOperationBehavior;
                        if (dataContractSerializerOperationBehavior != null)
                        {
                            dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = int.MaxValue;
                        }
                    }
                   }");

            foreach (MethodInfo[] methods in listMehtodInfo)
            {
                foreach (MethodInfo method in methods)
                {
                    builder.AppendLine(AssemblyGeneratorHelper.GenerateMethodDeclaration(method));
                    builder.AppendLine("{");
                    if (method.ReturnType != typeof(void))
                        builder.Append("return ");
                    builder.Append("base.Channel." + method.Name + "(");
                    builder.Append(AssemblyGeneratorHelper.GetMethodParameterString(method));
                    builder.AppendLine(");");
                    builder.AppendLine("}");
                }
            }
            if (wrapper)
                builder.AppendLine("public System.Boolean Ping(){return base.Channel.Ping();}");

            builder.AppendLine("} }");
            return builder.ToString();
        }

        public static void CreateProxy_Source(StringBuilder builder, string impClsName, string intName)
        {
            builder.AppendLine(@"public ClientBase<" + intName + @"> CreateProxy() {
                    return new " + impClsName + @"(wcfClientConfig.ABC.Binding,new StrategyContext(""random"").GetResult(wcfClientConfig.ServiceStatus, String.Empty));
                }");
        }

        public static string GenerateWcfFactorySource(List<MethodInfo[]> listMehtodInfo, string impClsName, string impFactoryClsName, string intName, bool wrapper, WcfClientConfig config)
        {
            StringBuilder builder = new StringBuilder(8192);
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ServiceModel;");
            builder.AppendLine("using System.ServiceModel.Description;");
            builder.AppendLine("using Dorado.ESB.ClientProxyFactory.Proxy;");
            builder.AppendLine("using Dorado.ESB.ClientProxyFactory.Config;");
            builder.AppendLine("using Dorado.Configuration;");
            builder.AppendLine("");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Threading;");
            builder.AppendLine("using System.Diagnostics;");
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text.RegularExpressions;");
            builder.AppendLine("namespace " + NameSpace + "{");

            builder.AppendLine("public class " + impFactoryClsName + " : IWcfClientFactory<" + intName + ">," + intName + " {");

            builder.AppendLine("private Dorado.ESB.ClientProxyFactory.Config.WcfClientConfig wcfClientConfig;");
            builder.AppendLine("private Thread heartbeatThread;");
            builder.AppendLine("private Stopwatch minDelayStopwatch;private string heartbeatTag = System.Guid.NewGuid().ToString();");
            builder.AppendLine("private static readonly Dorado.Logger.LoggerWrapper logger = new Dorado.Logger.LoggerWrapper();");
            builder.AppendLine("private static Dictionary<string, Type> dictionaryCache = new Dictionary<string, Type>();");
            builder.AppendLine("private static System.Timers.Timer timerHeartbeat;");
            builder.AppendLine(String.Format("private IDictionary<string, WcfProxyPool<{0}>> poolList = new Dictionary<string, WcfProxyPool<{1}>>();", intName, intName));

            Constructor_Source(builder, impFactoryClsName);
            Destructors_Source(builder, impFactoryClsName);
            CreateProxy_Source(builder, impClsName, intName);
            CreateChannelPool_Source(builder, impFactoryClsName, intName);
            InitDictionaryCache_Source(builder);
            PingAllServerRefreshLocalServiceStatus_Source(builder);
            RefreshLocalServiceStatus_Source(builder);
            TimerHeartbeat_Elapsed_Source(builder);
            InitTimerHeartbeat_Source(builder);
            CreateHeartbeat_Source(builder);
            CreateDictionaryCache_Source(builder);
            GetEndpointAddressFromStrategyContext_Source(builder);
            GetProxyFromPool_Source(builder, impClsName);
            StopServices_Source(builder);
            builder.AppendLine();

            int failedStrategyStatus = 0; //none
            if (String.Compare(config.NodeStrategy.FailedStrategy, "ImmediatelySwitch", StringComparison.CurrentCultureIgnoreCase) == 0)
                failedStrategyStatus = 1;//switch
            else if (String.Compare(config.NodeStrategy.FailedStrategy, "TryToSwitch", StringComparison.CurrentCultureIgnoreCase) == 0)
                failedStrategyStatus = 2;//try

            bool defaultCallLocal = false;
            if (String.Compare(config.ProviderStrategy.NotConfiguredStrategy, "Local", StringComparison.CurrentCultureIgnoreCase) == 0)
                defaultCallLocal = true;
            else if (String.Compare(config.ProviderStrategy.NotConfiguredStrategy, "Remote", StringComparison.CurrentCultureIgnoreCase) == 0)
                defaultCallLocal = false;

            if (config.ControlLevel.CompareTo("Whole") == 0 && config.UseLocalService == false)
            {
                listMehtodInfo.ForEach(delegate(MethodInfo[] methods)
                {
                    methods.ToList().ForEach(delegate(MethodInfo method)
                    {
                        builder.AppendLine(AssemblyGeneratorHelper.GenerateMethodDeclaration(method));
                        LoadBalanceStrategy(impClsName, intName, builder, failedStrategyStatus, method);
                    }
                );
                });
            }
            else
            {
                listMehtodInfo.ForEach(delegate(MethodInfo[] methods)
                {
                    methods.ToList().ForEach(delegate(MethodInfo method)
                    {
                        builder.AppendLine(AssemblyGeneratorHelper.GenerateMethodDeclaration(method));
                        KeyValuePair<string, bool> methodKeyValuePair = config.ProviderStrategy.DictionaryMethod.SingleOrDefault(c => c.Key == method.Name.ToLower());
                        if (methodKeyValuePair.Key != null)
                        {
                            if (methodKeyValuePair.Value)

                                //local
                                GenerateWcfMethodSource_Source_Local(builder, impClsName, intName, method);
                            else

                                //remote
                                LoadBalanceStrategy(impClsName, intName, builder, failedStrategyStatus, method);
                        }
                        else
                        {
                            if (defaultCallLocal)

                                //local
                                GenerateWcfMethodSource_Source_Local(builder, impClsName, intName, method);
                            else

                                //remote
                                LoadBalanceStrategy(impClsName, intName, builder, failedStrategyStatus, method);
                        }
                    }
                );
                });
            }

            if (wrapper)
                GeneratePing_Source(builder, impClsName);

            builder.AppendLine("}}");
            return builder.ToString();
        }

        private static void LoadBalanceStrategy(string impClsName, string intName, StringBuilder builder, int failedStrategyStatus, MethodInfo method)
        {
            if (failedStrategyStatus == 1)
                GenerateWcfMethodSource_Source_Switch(builder, impClsName, intName, method);
            else if (failedStrategyStatus == 2)
                GenerateWcfMethodSource_Source_Try(builder, impClsName, intName, method);
            else
                GenerateWcfMethodSource_Source_None(builder, impClsName, intName, method);
        }
    }
}