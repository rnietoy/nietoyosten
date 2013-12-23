﻿using System;
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
            if (email.Length >= 6 && password.Length >= 6 && password.Equals(confirm))
            {
                try
                {
                    result.UserID = this.Insert(new { Email = email, HashedPassword = Crypto.HashPassword(password) });
                    result.Success = true;
                    result.Message = "Thanks for signing up!";
                }
                catch (SqlException ex)
                {
                    result.Message = "This email already exists in our system";
                }
            }
            return result;
        }
    }
}