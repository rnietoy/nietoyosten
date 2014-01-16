using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Massive;

namespace NietoYostenMvc.Models
{
    public class Users : DynamicModel
    {
        public Users() : base("NietoYostenDb", "Users", "ID") {}

        public dynamic Register(string email, string password, string confirm)
        {
            dynamic result = new ExpandoObject();
            result.Success = false;

            if (email.Length < 6)
            {
                result.Message = "Email should be longer than 6 characters.";
                return result;
            }

            if (password.Length < 6)
            {
                result.Message = "Password should be longer than 6 characters.";
                return result;
            }

            if (!password.Equals(confirm))
            {
                result.Message = "Passwords do not match.";
                return result;
            }

            try
            {
                result.UserID = this.Insert(new { Email = email, HashedPassword = Crypto.HashPassword(password) });
                result.Success = true;
                result.Message = "Thanks for signing up!";
            }
            catch (SqlException ex)
            {
                result.Message = "This email already exists in our system.";
            }

            return result;
        }

        public void SetPassword(int UserID, string password)
        {
            this.Update(new {HashedPassword = Crypto.HashPassword(password)}, UserID);
        }

        public string GetUserRole(string email)
        {
            dynamic user = this.Single(where: "Email = @0", args: email);
            return user.Role;
        }

        public bool IsUserInRole(string email, string role)
        {
            return role.Equals(GetUserRole(email));
        }
    }
}