namespace NietoYostenMvc.Code
{
    public interface IFacebookApi
    {
        string GetUserEmail(string accessToken);
    }
}
