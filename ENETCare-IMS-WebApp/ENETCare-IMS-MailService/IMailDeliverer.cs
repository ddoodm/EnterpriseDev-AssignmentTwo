using System.Threading.Tasks;

namespace ENETCare.IMS.MailService
{
    internal interface IMailDeliverer
    {
        Task SendMail(string to, string from, string subject, string htmlBody);
    }
}