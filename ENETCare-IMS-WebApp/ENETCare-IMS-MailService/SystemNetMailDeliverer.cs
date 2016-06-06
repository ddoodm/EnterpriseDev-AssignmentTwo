using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ENETCare.IMS.MailService
{
    class SystemNetMailDeliverer : IMailDeliverer
    {
        public void SendMail(string to, string from, string subject, string htmlBody)
        {
            var mailMessage = new MailMessage();

            mailMessage.To.Add(to);
            mailMessage.From = new MailAddress(from, from);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlBody;
            mailMessage.IsBodyHtml = true;

            // Deliver the message
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Send(mailMessage);
            }
        }
    }
}
