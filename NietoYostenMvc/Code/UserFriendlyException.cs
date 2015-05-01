using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NietoYostenMvc.Code
{
    public class UserFriendlyException : Exception
    {
        private UserFriendlyException()
        {
            // do nothing
        }

        private UserFriendlyException(string message)
            : base(message)
        {
            // do nothing
        }

        private UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {
            // do nothing
        }

        public static UserFriendlyException GetInstance(string message)
        {
            return new UserFriendlyException(message);
        }

        public static UserFriendlyException GetInstance(string message, Exception innerException)
        {
            return new UserFriendlyException(message, innerException);
        }
    }
}