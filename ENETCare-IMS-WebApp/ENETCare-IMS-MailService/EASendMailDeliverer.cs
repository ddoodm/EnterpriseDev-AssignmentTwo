using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EASendMail;

namespace ENETCare.IMS.MailService
{
    /// <summary>
    /// Delivers mail using the EASendMail API
    /// </summary>
    internal class EASendMailDeliverer : IMailDeliverer
    {
        private const string EASENDMAIL_LICENSE_CODE = "TryIt";

        public async Task SendMail(string to, string from, string subject, string htmlBody)
        {
            var mailMessage = new SmtpMail(EASENDMAIL_LICENSE_CODE);
            var smtpClient = new SmtpClient();
            var smtpServer = new SmtpServer("");

            // Configure message
            mailMessage.From = from;
            mailMessage.To = to;
            mailMessage.Subject = subject;
            mailMessage.HtmlBody = htmlBody;

            // Deliver the message
            smtpClient.SendMail(smtpServer, mailMessage);
        }

        public bool TrySendMail(string to, string from, string subject, string htmlBody)
        {
            try
            {
                SendMail(to, from, subject, htmlBody);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
