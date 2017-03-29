namespace Dorado.Core.Email
{
    public static class EmailSenderExtensions
    {
        public static bool SendEmail(this IEmailSender sender, EmailAccount account, string subject, string body, string from, params string[] to)
        {
            try
            {
                EmailMessage emailMessage = new EmailMessage();
                foreach (var address in to)
                {
                    emailMessage.To.Add(new EmailAddress(address));
                }

                emailMessage.Subject = subject;
                emailMessage.Body = body;

                sender.SendEmail(new SmtpContext(account), emailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}