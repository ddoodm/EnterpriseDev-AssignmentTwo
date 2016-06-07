using System.Threading.Tasks;

namespace ENETCare.IMS.MailService
{
    public interface IMailDeliverer
    {
        void SendMail(string to, string from, string subject, string htmlBody);
    }
}