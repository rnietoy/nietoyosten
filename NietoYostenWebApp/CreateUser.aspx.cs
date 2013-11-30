using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Web.Security;

namespace NietoYostenWebApp
{
    public partial class CreateUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            // Send an email to admins notifying that there are pending users
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress("noreply@nietoyosten.com");
            message.From = fromAddress;

            string[] admins = Roles.GetUsersInRole("admin");
            foreach (string admin in admins)
            {
                message.To.Add(Membership.GetUser(admin).Email);
            }

            message.Subject = "New user(s) pending approval";
            message.Body = "A new user has been registered on the Nieto Yosten web site. " +
                "Please log in to http://nietoyosten.com, go to the Admin section, and then " +
                "go to Users -> Pending Users.\n";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }
    }
}