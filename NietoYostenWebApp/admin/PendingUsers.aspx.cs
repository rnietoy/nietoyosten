using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Net.Mail;

namespace NietoYostenWebApp.admin
{
    public partial class PendingUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateGrid();   
            }   
        }

        void UpdateGrid()
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            var q =
                    from m in db.aspnet_Memberships
                    where m.IsApproved == false
                    select new { UserName = m.aspnet_User.UserName, m.Email };
            PendingUsersGrid.DataSource = q;
            PendingUsersGrid.DataBind();
        }

        protected void PendingUsersGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = PendingUsersGrid.Rows[index];
            TableCell userNameCell = selectedRow.Cells[0];
            string userName = userNameCell.Text;
            MembershipUser user = Membership.GetUser(userName);
            if (e.CommandName == "Approve")
            {
                user.IsApproved = true;
                if (!Roles.IsUserInRole(userName, "friend"))
                {
                    Roles.AddUserToRole(userName, "friend");
                }
                Membership.UpdateUser(user);
                NotifyApproved(user.Email, user.UserName);
            }
            else if (e.CommandName == "Reject")
            {
                Membership.DeleteUser(userName);
            }
            UpdateGrid();
        }

        void NotifyApproved(string email, string userName)
        {
            // Send an email to an user notifying that he/she has been approved
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress("noreply@nietoyosten.com");
            message.From = fromAddress;
            message.To.Add(email);
            message.Subject = "Your account has been approved";
            message.Body = "<p>Thank you for creating your account at nietoyosten.com! The account <strong>" +
                HttpUtility.HtmlEncode(userName) + "</strong> " +
                "has been approved. You may now go to " +
                "<a href=\"http://www.nietoyosten.com\">http://www.nietoyosten.com</a> and log in " +
                "to visit our site.</p>" +
                "<p>Best Regards,</p>" +
                "The Nieto Yosten Family";
            message.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }
    }
}