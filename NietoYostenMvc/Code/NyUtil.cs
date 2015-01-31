using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Microsoft.WindowsAzure.Storage;

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

        public static CloudStorageAccount StorageAccount
        {
            get
            {
                const string account = "nietoyosten";
                string key = ConfigurationManager.AppSettings["STORAGE_ACCOUNT_KEY"];
                string connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}