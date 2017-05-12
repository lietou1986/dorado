/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/12 15:19:08
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

//using Dorado.VWS.Admin.PermissionWCF;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public class WebBasePage : Page
    {
        protected string CurUserName = string.Empty;
        protected IList<DomainPermissionEntity> domainPermissions;
        protected IList<DomainEntity> domains;
        private readonly ServerProvider _serverProvider = new ServerProvider();
        protected static CookieManager cookieMgr = new CookieManager();

        /// <summary>
        /// 当前用户是否为管理员
        /// </summary>
        public static bool CurUserIsAdmin
        {
            get
            {
                string adminStr = cookieMgr.getCookieValue("CurUserIsAdmin");
                return !string.IsNullOrEmpty(adminStr) && bool.Parse(adminStr);
            }
            set
            {
                cookieMgr.setCookie("CurUserIsAdmin", value.ToString());
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            //CookieManager cookieMgr=new CookieManager();
            return cookieMgr.getCookieValue("UserName");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CurUserName = GetUserName();
            DomainPermissionProvider _domianPMSProvider = new DomainPermissionProvider();
            domainPermissions = _domianPMSProvider.GetDomainsByUser(CurUserName);
        }

        protected void ShowAlert(string message)
        {
            Page.ClientScript.RegisterStartupScript(typeof(string), "ShowAlert",
                                                    " alert(\"" + message + "\");", true);
        }

        [Obsolete("ShowAlter 拼写错误，请使用 ShowAlert")]
        protected void ShowAlter(string message)
        {
            //#warning 警告：错误的命名，请使用ShowAlert
            ShowAlert(message);
        }

        protected void ShowAlertAndRedirect(string message, string page)
        {
            //Page.ClientScript.RegisterStartupScript(typeof(string), "ShowAlert", "alert(\"" + message + "\");window.location.href=\"" + page + "\"", true);
            Response.Write("<script type=\"text/javascript\" >alert(\"" + message + "\");window.location.href=\"" + page + "\";</script>");
            Response.End();
        }

        protected void ShowAlertAndBack(string message)
        {
            //Page.ClientScript.RegisterStartupScript(typeof(string), "ShowAlert", "alert(\"" + message + "\");history.go(-1);", true);
            Response.Write("<script type=\"text/javascript\" >alert(\"" + message + "\");history.go(-1);</script>");
            Response.End();
        }

        protected override void OnError(EventArgs e)
        {
            Exception ex = Server.GetLastError();
            ShowAlert(ex.ToString());
            base.OnError(e);
        }

        protected void RedirectNewWindow(string page)
        {
            string redirect = string.Format("window.open('{0}');", page, true);
            Page.ClientScript.RegisterStartupScript(typeof(string), "open", redirect);
        }

        /// <summary>
        ///     判断是否有资源权限
        /// </summary>
        /// <param name = "domainId">域名ID</param>
        /// <param name = "resouceValue">资源值</param>
        /// <returns>结果</returns>
        [Obsolete("旧的资源权限判断")]
        protected bool HasResoucePermission(int domainId, string resouceValue)
        {
            var pmsProvider = new PermissionProvider();
            return pmsProvider.HasResoucePermission(CurUserName, domainId, resouceValue);
        }

        /// <summary>
        /// 判断用户是否具有域名权限
        /// </summary>
        /// <param name="domainID"></param>
        /// <param name="manageType"></param>
        /// <returns></returns>
        public bool HasDomainPermission(int domainID, EnumManageType manageType)
        {
            if (CurUserIsAdmin)
            {
                return true;
            }
            var domainPmsProvider = new DomainPermissionProvider();
            //var users = domainPmsProvider.GetUsersByDomainAndPermissionType(domainID, (int)manageType);
            //if (users.Count > 0)
            //{
            //    return users.Any(domainPermission => domainPermission.UserName.Equals(CurUserName));

            //}
            return domainPmsProvider.HasPermission(domainID, CurUserName, (int)manageType);
            //return false;
        }

        public static bool HasDomainPermission(string username, int domainID, EnumManageType manageType)
        {
            if (CurUserIsAdmin)
            {
                return true;
            }
            var domainPmsProvider = new DomainPermissionProvider();
            //var users = domainPmsProvider.GetUsersByDomainAndPermissionType(domainID, (int)manageType);
            //if (users.Count > 0)
            //{
            //    return users.Any(domainPermission => domainPermission.UserName.Equals(CurUserName));

            //}
            return domainPmsProvider.HasPermission(domainID, username, (int)manageType);
            //return false;
        }

        public IList<DomainEntity> GetUserDomians(params EnumManageType[] manageType)
        {
            List<int> pid = new List<int> { };
            foreach (var m in manageType)
            {
                pid.Add((int)m);
            }
            IList<DomainEntity>
                domainList = _serverProvider.GetDomainsByIdcId(0);
            if (CurUserIsAdmin)
            {
                return domainList;
            }
            else
            {
                var query = from DomainEntity domain in domainList
                            where (
                                    from DomainPermissionEntity permission in domainPermissions
                                    where pid.Contains(permission.PermissionType)
                                    select permission.DomainID
                                    )
                                    .Contains(domain.DomainId)
                            select domain;

                return query.ToList();
            }
        }

        public IList<DomainEntity> GetUserDomians(string environment, params EnumManageType[] manageType)
        {
            List<int> pid = new List<int> { };
            foreach (var m in manageType)
            {
                pid.Add((int)m);
            }
            IList<DomainEntity>
                domainList = _serverProvider.GetDomainsByIdcId(0);
            if (CurUserIsAdmin)
            {
                var query = from DomainEntity domain in domainList
                            where domain.Environment == environment
                            select domain;
                return query.ToList();
            }
            else
            {
                var query = from DomainEntity domain in domainList
                            where (
                                    from DomainPermissionEntity permission in domainPermissions
                                    where pid.Contains(permission.PermissionType)
                                    select permission.DomainID
                                    )
                                    .Contains(domain.DomainId) && domain.Environment == environment
                            select domain;

                return query.ToList();
            }
        }

        public IList<DomainEntity> GetUserDomians(EnumManageType manageType, string environment)
        {
            List<int> pid = new List<int> { };

            pid.Add((int)manageType);

            IList<DomainEntity>
                domainList = _serverProvider.GetDomainsByIdcId(0);
            if (CurUserIsAdmin)
            {
                var query = from DomainEntity domain in domainList
                            where domain.Environment == environment
                            select domain;
                return query.ToList();
            }
            else
            {
                var query = from DomainEntity domain in domainList
                            where (
                                    from DomainPermissionEntity permission in domainPermissions
                                    where pid.Contains(permission.PermissionType)
                                    select permission.DomainID
                                    )
                                    .Contains(domain.DomainId) && domain.Environment == environment
                            select domain;

                return query.ToList();
            }
        }

        [Obsolete("应当输入指定权限的参数")]
        public IList<DomainEntity> GetUserDomians()
        {
            return GetUserDomians(EnumManageType.DailyManage);
        }

        /// <summary>
        /// 用户可以授权的权限
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetManagePermission()
        {
            var list = new Dictionary<int, string>();

            EnumManageType[] ManageTypes = (EnumManageType[])Enum.GetValues(typeof(Model.Enum.EnumManageType));
            foreach (EnumManageType permission in ManageTypes)
            {
                list.Add((int)permission, EnumHelper.GetDescription(permission));
            }
            if (!CurUserIsAdmin)
            {
                list.Remove((int)EnumManageType.DailyManage);
            }

            return list;
        }
    }
}