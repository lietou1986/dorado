using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelloWorldApp.Contracts.ServiceContracts;
using System.ServiceModel;
using HelloWorldApp.Contracts.DataContracts;
using Beisen.ESB.Extensions.Behaviors;
using System.Threading;
using Beisen.ESB.Extensions;
namespace HelloWorldApp.Library
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[Pooling(MinPoolSize = 0, MaxPoolSize = 100)]
    public class HelloWorldService:IHelloWorldService
    {
        public HelloWorldService()
        {
            //Thread.Sleep(5000);
        }


        #region IHelloWorldService 成员
        [OperationBehavior]
        [CacheManager(5)]
        public string GetHelloWorldApp()
        {
            return "Hello World by Project HelloWorldApp!";
        }
        [OperationBehavior]
        public UserInfo GetUserInfo(string userName)
        {
            UserInfo userInfo = new UserInfo();
            switch (userName.ToLower())
            {
                case "ceping":
                    userInfo.UserName = userName;
                    userInfo.Sex = "Man";
                    break;
                default:
                    userInfo.UserName = userName;
                    userInfo.Sex = "WoMan";
                    break;

            }
            return userInfo;
        }

        public UserInfo GetUserInfo5(string userName)
        {
            UserInfo userInfo = new UserInfo();
            switch (userName.ToLower())
            {
                case "ceping":
                    userInfo.UserName = userName;
                    userInfo.Sex = "Man";
                    break;
                default:
                    userInfo.UserName = userName;
                    userInfo.Sex = "WoMan";
                    break;

            }
            return userInfo;
        }



        public bool Ping()
        {
            return true;
        }

        #endregion

        #region IHelloWorldService Members


        //public bool Ping(int x)
        //{
        //    return true;
        //}

        //public bool Ping(string y)
        //{
        //    return true;
        //}

        #endregion

        #region IHelloWorldService Members



        public UserInfo SetUserInfo2(UserInfo ui)
        {

            UserInfo userInfo = new UserInfo();
            userInfo.UserName = ui.UserName;
            userInfo.myState = State.No;
            userInfo.Sex = ui.Sex;

            return userInfo;
        }


        public UserInfo SetUserInfo3(UserInfo ui, string country,Test test)
        {

            UserInfo userInfo = new UserInfo();
            userInfo.UserName = ui.UserName;
            userInfo.Sex = ui.Sex;
            userInfo.Info = country + "|" + test.Name;


            return userInfo;
        }

        public UserInfo SetUserInfo4(UserInfo ui, string country)
        {

            UserInfo userInfo = new UserInfo();
            userInfo.UserName = ui.UserName;
            userInfo.Sex = ui.Sex;
            userInfo.Info = country;

            return userInfo;
        }


        #endregion

        #region IHelloWorldService Members


        public UserInfo SetUserInfo5(List<UserInfo> userinfoList, State state,DateTime dt)
        {

            UserInfo ui = new UserInfo();
            ui.UserName = "leiming" + "|" + dt.ToString();
            ui.myState = state;
            ui.Books = new List<Book>(new Book [] { new Book{ Name="yw"},new Book{ Name="ss"}});
            return ui;
        }


        public DateTime SetUserInfo6(DateTime dt)
        {
            return dt;



        }
        public State SetUserInfo7(State st)   
        {
            return st;



        }
        #endregion
    }
}
