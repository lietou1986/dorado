using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloWorldApp.Contracts.ServiceContracts;
using HelloWorldApp.Contracts.DataContracts;
using System.ServiceModel;

namespace HelloWorldApp.Library
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class HelloBeisenService : IHelloBeisenService
    {
        public State SetUserInfo8(State st)
        {
            return st;



        }
    }
}
