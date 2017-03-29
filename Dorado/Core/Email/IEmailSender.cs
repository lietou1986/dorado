using System.Threading.Tasks;

namespace Dorado.Core.Email
{    
    /// <summary>
    /// contract for email sender 
    /// </summary>
    public interface IEmailSender
    {
        void SendEmail(SmtpContext smtpContext, EmailMessage message);
		Task SendEmailAsync(SmtpContext smtpContext, EmailMessage message); 
    }
}
