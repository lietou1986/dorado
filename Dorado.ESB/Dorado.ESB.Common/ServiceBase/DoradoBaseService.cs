using System;
using System.Configuration;
using System.ServiceModel;

namespace Dorado.ESB.Common
{
    /// <summary>Base Service - all services inherit from this class.</summary>
    ///
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DoradoBaseService : MarshalByRefObject, IDisposable
    {
        #region Ctor

        #endregion Ctor

        #region IDisposable Members

        public void Dispose()
        {
        }

        //public bool CheckUserToken(string Token)
        //{
        //    bool ret = false;
        //    try
        //    {
        //        if (Token != null)
        //        {
        //            MyUserInfo userInfo = new MyUserInfo(HttpUtility.UrlDecode(Token).Replace(' ', '+'), true);
        //            if (null != userInfo && userInfo.UserID > 0)
        //                ret = true;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ServiceLog.Error(ex.ToString(), ex);
        //        ret = false;
        //    }
        //    return ret;
        //}

        //public bool CheckUserCookie()
        //{
        //    bool ret = false;
        //    try
        //    {
        //        HttpCookie myUserInfoCookie = Dorado.Application.Web.Context.RequestContext.Instance.GetCookieFromContext(CookieKeys.USER_INFO_COOKIE_KEY);
        //        if (myUserInfoCookie != null && myUserInfoCookie.Value.Length > 0)
        //        {
        //            MyUserInfo userInfo = new MyUserInfo(HttpUtility.UrlDecode(myUserInfoCookie.Value).Replace(' ', '+'), true);
        //            if (null != userInfo && userInfo.UserID > 0)
        //                ret = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ServiceLog.Error(ex.ToString(), ex);
        //        ret = false;
        //    }
        //    return ret;
        //}

        public bool CheckUserList(int userID)
        {
            string checkUser = ConfigurationManager.AppSettings["CheckUser"].ToString();
            bool isEx = false;
            if (checkUser.ToLower() == "enabled")
            {
                string userIDs = ConfigurationManager.AppSettings["AllowUserIDs"].ToString();
                string[] userList = userIDs.Split(new Char[] { '|' });

                for (int i = 0; i < userList.Length; i++)
                {
                    if (userID == int.Parse(userList[i]))
                    {
                        isEx = true;
                        break;
                    }
                }
            }
            else
            {
                isEx = true;
            }

            return isEx;
        }

        #endregion IDisposable Members
    }
}