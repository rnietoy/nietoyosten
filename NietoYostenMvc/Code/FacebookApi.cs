using System;
using Facebook;

namespace NietoYostenMvc.Code
{
    public class FacebookApi : IFacebookApi
    {
        public string GetUserEmail(string accessToken)
        {
            dynamic user;
            try
            {
                FacebookClient fbClient = new FacebookClient(accessToken);
                user = fbClient.Get("me");
            }
            catch (FacebookApiException)
            {
                return null;
            }
            return user.email;
        }
    }
}