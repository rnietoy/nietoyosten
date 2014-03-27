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
        private enum Role
        {
            Admin,
            Family,
            Friend
        };

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
                result.User = this.Insert(new { Email = email, HashedPassword = Crypto.HashPassword(password) });
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

        /// <summary>
        /// Check if a given user has the level of access specified by the given role.
        /// For example, if the role argument is "family" and the user's role is "admin", then
        /// this is will return true, since the "admin" role is a higher level of access than "family".
        /// </summary>
        /// <param name="email">Email of user whose role will be checked</param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool UserHasRole(string email, string role)
        {
            if (null == role) return false;

            string userRole = GetUserRole(email);
            if (null == userRole) return false;

            int userRoleLevel = (int) Enum.Parse(typeof (Role), GetUserRole(email), true);
            int expectedRoleLevel = (int) Enum.Parse(typeof (Role), role, true);

            return userRoleLevel <= expectedRoleLevel;
        }

        public IEnumerable<string> GetUsersInRole(string role)
        {
            var result = this.All(columns: "Email", where: "Role=@0", args: role);
            return result.Select(x => (string)x.Email);
        }
    }
}