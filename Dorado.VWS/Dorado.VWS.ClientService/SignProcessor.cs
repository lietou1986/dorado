using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vancl.IC.VWS.ClientService
{
    public class SignProcessor : ProcessAbstract
    {
        public override void DoWork()
        {
            HandlerHelper.SignHandler(ClientConst.LocalIP, ClientConst.ClientVersion, ClientConst.HostName, Common.IISStatus()); 
        }
    }
}
