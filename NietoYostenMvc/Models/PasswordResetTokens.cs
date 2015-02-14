using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Massive;

namespace NietoYostenMvc.Models
{
    public class PasswordResetTokens : DynamicModel
    {
        public PasswordResetTokens() : base("NietoYostenDb", "PasswordResetTokens", "ID") { }

        public string AddToken(int userId)
        {
            string token = Guid.NewGuid().ToString().Replace("-", "");
            this.Insert(new {HashedToken = Crypto.Hash(token), UserID = userId});
            return token;
        }
    }
}