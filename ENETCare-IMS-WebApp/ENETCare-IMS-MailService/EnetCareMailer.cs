using ENETCare.IMS.Interventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using ENETCare.IMS.MailService.Properties;
using ENETCare.IMS.Users;

namespace ENETCare.IMS.MailService
{
    public static class EnetCareMailer
    {
        private const string
            HTML_HEADING_LOCATION_TAG = "@HEADING_LOCATION@",
            HTML_MESSAGE_LOCATION_TAG = "@MESSAGE_LOCATION@";

        /// <returns>A concrete instance of an IMailDeliverer</returns>
        private static IMailDeliverer GetMailDeliverer()
        {
            return new SystemNetMailDeliverer();
        }

        private static string FormatMessageIntoLayout(string heading, string message)
        {
            string htmlLayoutFormat = Resources.EmailBodyFormat;
            return htmlLayoutFormat
                .Replace(HTML_HEADING_LOCATION_TAG, heading)
                .Replace(HTML_MESSAGE_LOCATION_TAG, message);
        }

        private static string InterventionStateChangeMessage(Intervention intervention)
        {
            string messageFormat = Resources.InterventionStateChangeMessageFormat;
            return String.Format(
                messageFormat,
                intervention.ApprovalState,
                intervention.InterventionType,
                intervention.Client.DescriptiveName);
        }

        /// <summary>
        /// Mails the Site Engineer who proposed the intervention
        /// regarding a state change.
        /// </summary>
        /// <param name="intervention"></param>
        public static async Task NotifyEngineerOfStateChange(Intervention intervention)
        {
            var siteEngineer = intervention.SiteEngineer;

            string from = "service@enet-care.com.au";
            string subject = "An Intervention Has Changed State";
            string message = InterventionStateChangeMessage(intervention);

            string to = siteEngineer.Email;

            string htmlMessage = FormatMessageIntoLayout(subject, message);

            IMailDeliverer mailer = GetMailDeliverer();
            await mailer.SendMail(to, from, subject, htmlMessage);
        }
    }
}
