using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace NietoYostenMvc.Code
{
    public class AlertClass
    {
        public const string AlertSuccess = "alert-success";
        public const string AlertDanger = "alert-danger";
        public const string AlertInfo = "alert-info";
    }

    public class NyResult
    {
        public bool Success;
        public string RedirectUrl;
        public string Message;
    }

    public static class NyUtil
    {
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

        /// <summary>
        /// Convert a flat collection of elements into a table represented by a collection of collections.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection</typeparam>
        /// <param name="elements">The collection of elements</param>
        /// <param name="nColumns">Number of columns in the array</param>
        /// <returns>The given elements arranged in a two dimensional array</returns>
        public static IEnumerable<IEnumerable<T>> ConvertListToTable<T>(IEnumerable<T> elements, int nColumns)
        {
            var result = new List<List<T>>();
            int colPos = 0;
            List<T> currentRow = new List<T>();

            foreach (var item in elements)
            {
                currentRow.Add(item);
                colPos++;
                if (colPos > 3)
                {
                    result.Add(currentRow);
                    colPos = 0;
                    currentRow = new List<T>();
                }
            }
            if (currentRow.Any())
            {
                result.Add(currentRow);
            }

            return result;
        }
    }
}