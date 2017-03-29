using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Beisen.Configuration.Logging;

namespace Beisen.ESB.ClientProxyFactory.Proxy
{
    public class WcfSelectAddress
    {
        private static object AddressStatusListLock = new object();


        //---------Add WcfSelectAddress File By Guantao 2009-11-27 multiAddressHeartbeat
        public static EndpointAddress GetSelectAddress(string interfaceName, ServiceStatus serviceStatus)
        {
            
            EndpointAddress endpointAddress = null;
            try
            {
                List<AddressStatus> addressStatusList;
                lock (AddressStatusListLock)
                {
                    addressStatusList= serviceStatus.AddressStatusList;
                    List<string> availableAddressList = new List<string>();
                    List<string> unAvailableAddressList = new List<string>();
                    foreach (AddressStatus addressStatus in addressStatusList)
                    {
                        if (addressStatus.ServiceInterface.ToLower() == interfaceName.ToLower() && addressStatus.AddressOK)
                        {
                            availableAddressList.Add(addressStatus.Address);

                        }
                        else
                        {
                            unAvailableAddressList.Add(addressStatus.Address);
                        }
                    }
                    if (availableAddressList.Count > 0)
                    {
                        Random random = new Random();
                        int val = random.Next(1, availableAddressList.Count + 1);
                        endpointAddress = new EndpointAddress(availableAddressList[val - 1]);

                    }
                    else
                    {

                        Random random = new Random();
                        int val = random.Next(1, unAvailableAddressList.Count + 1);
                        endpointAddress = new EndpointAddress(unAvailableAddressList[val - 1]);

                        //2010-04-10 by guantao
                        //如果当前地址（总接口）状态都是false，目前endpointAddres返回为null
                        //如果都为false，则应该随机挑选一个为false的endpointAddres，以供的自主建立，而不应该返回null.

                        //记录错误日志
                        LoggingWrapper.HandleException(new Exception("A serious warning, all the wcf server error"), "All WCF Server Error");
                    }
                }
                
               
            }
            catch (Exception ex)
            {
                LoggingWrapper.HandleException(ex, "Select WCF Address Error：" + interfaceName);

            }
            return endpointAddress;
        }
    }
}
