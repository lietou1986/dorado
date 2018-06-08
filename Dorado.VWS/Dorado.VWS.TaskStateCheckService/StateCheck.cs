using System;
using System.Linq;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.TaskStateCheckService
{
    public class StateCheck
    {
        private static SynctaskDao syncTaskDao = new SynctaskDao();
        public static bool IsChecking = false;
        private static object locker = 0;
        private static System.Timers.Timer t;

        private static string mailTitle;
        private static string mailBody;
        private static string mailCC;
        private static int checkInterval;
        private static int[] noticHours;

        public static void Start()
        {
            check(null, null);
            t = new System.Timers.Timer(1000 * checkInterval);
            t.Elapsed += check;
            t.Enabled = true;
            t.Start();
        }

        public static void Stop()
        {
            t.Enabled = false;
            t.Stop();
        }

        public static void check(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!noticHours.Contains(DateTime.Now.Hour))
            {
                return;
            }
            lock (locker)
            {
                try
                {
                    IsChecking = true;

                    string sql = @"SELECT synctask.*,vwsdomain.DomainName FROM zsync_synctask synctask(NOLOCK)
                           join zsync_domain vwsdomain(NOLOCK) on synctask.DomainId = vwsdomain.Id
                           WHERE synctask.SyncStatus = 2 and synctask.DeleteFlag = 0
                           order by synctask.DomainId,synctask.CreateTime desc";
                    var tasks = syncTaskDao.GetEntities(System.Data.CommandType.Text, sql);

                    foreach (var task in tasks)
                    {
                        sendMail(task);
                    }

                    IsChecking = false;
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static void sendMail(SynctaskEntity task)
        {
            string title = string.Format(mailTitle, task.TaskId);
            string body = string.Format(mailBody, task.DomainName).Replace("&lt;", "<").Replace("&gt;", ">");
            string rec = task.UserName + "@dorado.com";
            string cc = "";//抄送列表
            var managers = GetManger(task.DomainId);//抄送列表增加域名管理
            foreach (string m in managers)
            {
                cc += m + "@dorado.com" + ",";
            }
            cc += mailCC;//抄送列表增加webconfig中配置的同步系统管理员
            //Common.MailSender.Send(System.Net.Mail.MailPriority.High, rec, cc, title, body, true, null);
        }

        private static string[] GetManger(int domainid)
        {
            DomainPermissionProvider permissionProvider = new DomainPermissionProvider();
            var permissionUsers = permissionProvider.GetUsersByDomainAndPermissionType(domainid, (int)EnumManageType.DailyManage);

            return (from DomainPermissionEntity user in permissionUsers
                    select user.UserName).ToArray();
        }
    }
}