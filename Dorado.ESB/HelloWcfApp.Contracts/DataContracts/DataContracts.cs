using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HelloWcfApp.Contracts.DataContracts
{
    [DataContract(Namespace = "http://www.beisen.com")]
    public class AccountInfo
    {
        [DataMember]
        public string UserName
        {
            get;
            set;
        }
        [DataMember]
        public int Account
        {
            get;
            set;
        }
    }
}
