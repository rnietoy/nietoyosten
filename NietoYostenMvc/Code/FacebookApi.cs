using Facebook;

namespace NietoYostenMvc.Code
{
    public class FacebookApi : IFacebookApi
    {
        public string GetUserEmail(string accessToken)
        {
            FacebookClient fbClient = new FacebookClient(accessToken);
            dynamic user = fbClient.Get("me");
            return user.email;
        }
    }
}