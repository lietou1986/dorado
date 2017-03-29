using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HelloWorldApp.Contracts.DataContracts
{
    [DataContract(Namespace="http://www.beisen.com")]
    public class UserInfo
    {
        [DataMember]
        public string UserName
        {
            get;
            set;
        }
        [DataMember]
        public string Sex
        {
            get;
            set;
        }

        [DataMember]
        public string Info
        {
            get;
            set;
        }


        [DataMember]
        public List<Book> Books
        {
            get;
            set;
        }

        [DataMember]
        public State myState
        {
            get;
            set;
        }



    }

    [DataContract(Namespace = "http://www.beisen.com")]
    public enum State
    {
        [EnumMember(Value="MyOk")]
        Ok = 0,
        [EnumMember(Value = "MyNo")]
        No=1

    }
   
    [DataContract(Namespace = "http://www.beisen.com")]
    public class Book
    {
         [DataMember]
        public string Name
        {
            get;
            set;
        }
    }


    [DataContract(Namespace = "http://www.beisen.com")]
    public class Test
    {

        [DataMember]
        public string Name
        {
            get;
            set;
        }
    }
}
