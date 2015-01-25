using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace NietoYostenMvc.Code
{
    public class NyUtil
    {
        public static void SendMail(MailMessage message)
        {
            using (var smtpClient = new SmtpClient())
            {
                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.Network)
                {
                    smtpClient.Credentials = new NetworkCredential("nietoyosten", ConfigurationManager.AppSettings["mailer_pwd"]);
                }
                smtpClient.Send(message);
            }
        }
    }
}