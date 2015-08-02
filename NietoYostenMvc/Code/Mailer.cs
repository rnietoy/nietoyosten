using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace NietoYostenMvc.Code
{
    public interface IMailer
    {
        void SendMail(MailMessage message);
    }

    public class Mailer : IMailer
    {
        public void SendMail(MailMessage message)
        {
            using (var smtpClient = new SmtpClient())
            {
                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.Network)
                {
                    smtpClient.Credentials = new NetworkCredential("nietoyosten", ConfigurationManager.AppSettings["mailer_pwd"]);
                }
                message.Headers.Add("Message-ID", "<" + Guid.NewGuid().ToString().Replace("-", "") + "@mail.nietoyosten.com>");
                smtpClient.Send(message);
            }
        }
    }
}