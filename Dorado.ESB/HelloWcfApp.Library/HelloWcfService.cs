using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloWcfApp.Contracts.ServiceContracts;
using HelloWcfApp.Contracts.DataContracts;
using Beisen.ESB.Extensions.Behaviors;
using System.Threading;
using System.ServiceModel;
namespace HelloWcfApp.Library
{


    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [Pooling(MinPoolSize = 0, MaxPoolSize = 100)]
    public class HelloWcfService : IHelloWcfService
    {
        #region IHelloWcfService 成员

        public HelloWcfService()
        {
            Thread.Sleep(5000);

        }

        public string GetHelloWcfApp()
        {
            return "Hello Wcf by Project HelloWcfApp!";
        }

        public HelloWcfApp.Contracts.DataContracts.AccountInfo GetAccountInfo(string userName)
        {
            AccountInfo accountInfo = new AccountInfo();
            switch (userName.ToLower())
            {
                case "beisen":
                    accountInfo.UserName = userName;
                    accountInfo.Account = 1000;
                    break;
                default:
                    accountInfo.UserName = userName;
                    accountInfo.Account= 500;
                    break;
            }
            return accountInfo;
        }
        public bool Ping()
        {
            return true;
        }
        #endregion
    }
}
