using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;

namespace NietoYostenMvc.Code
{
    public class AlertClass
    {
        public const string AlertSuccess = "alert-success";
        public const string AlertDanger = "alert-danger";
        public const string AlertInfo = "alert-info";
    }

    public static class NyUtil
    {
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

        public static void SendMail(MailMessage message)
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

        public static void SetAlertMessage(this Controller controller, string message, string alertClass = AlertClass.AlertSuccess)
        {
            controller.TempData["AlertMessage"] = message;
            controller.TempData["AlertClass"] = alertClass;
        }
    }
}