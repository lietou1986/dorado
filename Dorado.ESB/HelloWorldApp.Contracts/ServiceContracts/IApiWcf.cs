﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace HelloWorldApp.Contracts.ServiceContracts
{
    [ServiceContract]
    
    public interface IWcfApi : IHelloWorldService,IHelloBeisenService
    {
     
    }

}
