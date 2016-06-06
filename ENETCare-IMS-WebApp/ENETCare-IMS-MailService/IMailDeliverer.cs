namespace ENETCare.IMS.MailService
{
    internal interface IMailDeliverer
    {
        void SendMail(string to, string from, string subject, string htmlBody);
        bool TrySendMail(string to, string from, string subject, string htmlBody);
    }
}