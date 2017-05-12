/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 9:50:33
 * 版本号：v1.0
 * 本类主要用途描述：发送邮件类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using Dorado.Core;
using Dorado.Core.Logger;

#endregion using

namespace Dorado.VWS.Services
{
    /// <summary>
    ///     默认使用UTF-8格式发送
    /// </summary>
    public class MailSender
    {
        #region 发送监控邮件

        public static void SendErrorEmail(string errorMessage)
        {
            try
            {
                var fromAddress = new MailAddress("len@dorado.com ", "service", Encoding.UTF8);
                var mailMsg = new MailMessage { From = fromAddress };

                string emailAddress = ConfigurationManager.AppSettings["EmailAddress"];
                foreach (string emailaddres in emailAddress.Split(';'))
                {
                    mailMsg.To.Add(new MailAddress(emailaddres));
                }
                mailMsg.Priority = MailPriority.High;
                mailMsg.Body = errorMessage;
                mailMsg.IsBodyHtml = true;
                mailMsg.BodyEncoding = Encoding.UTF8;
                mailMsg.Subject = "同步系统异常";
                mailMsg.SubjectEncoding = Encoding.UTF8;
                mailMsg.Sender = new MailAddress("len@dorado.com ", "service", Encoding.UTF8);
                var mailClient = new SmtpClient("smtps.dorado.com ", 25)
                                            {
                                                Credentials =
                                                    new NetworkCredential("len@dorado.com ", ".u8x9y1m4k7r3h6"),
                                                DeliveryMethod = SmtpDeliveryMethod.Network
                                            };
                mailClient.Send(mailMsg);
                LoggerWrapper.Logger.Info("VWS.Site", "监控邮件发送成功");
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Error("VWS.Site", string.Format("监控邮件发送失败：" + e.Message + e.StackTrace));
            }
        }

        #endregion 发送监控邮件
    }
}