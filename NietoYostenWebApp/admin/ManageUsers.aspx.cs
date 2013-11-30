using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace NietoYostenWebApp.admin
{
    public partial class ManageUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateGrid();
            }
        }

        class UserRow
        {
            string userName;
            string email;
            string roles;
            bool approved;

            public UserRow(string userName, string email, string roles, bool approved)
            {
                this.userName = userName;
                this.email = email;
                this.roles = roles;
                this.approved = approved;
            }

            public string UserName
            {
                get { return userName; }
            }

            public string Email
            {
                get { return email; }
            }

            public string Roles
            {
                get { return roles; }
            }

            public bool Approved
            {
                get { return approved; }
            }
        }

        void UpdateGrid()
        {
            List<UserRow> userList = new List<UserRow>();

            MembershipUserCollection users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                string roles = string.Join(", ", Roles.GetRolesForUser(user.UserName));
                userList.Add(new UserRow(user.UserName, user.Email, roles, user.IsApproved));
            }

            UsersGrid.DataSource = userList;
            UsersGrid.DataBind();
        }

        protected void UsersGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Get user name of row in which the button was clicked
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = UsersGrid.Rows[index];
            TableCell userNameCell = selectedRow.Cells[0];
            string userName = userNameCell.Text;
            MembershipUser user = Membership.GetUser(userName);

            switch (e.CommandName)
            {
                case "DeleteUser":
                    Membership.DeleteUser(userName);
                    UpdateGrid();
                    break;

                case "ApproveUser":
                    user.IsApproved = true;
                    Membership.UpdateUser(user);
                    UpdateGrid();
                    break;

                case "UnApproveUser":
                    user.IsApproved = false;
                    Membership.UpdateUser(user);
                    UpdateGrid();
                    break;
            }
        }
    }
}