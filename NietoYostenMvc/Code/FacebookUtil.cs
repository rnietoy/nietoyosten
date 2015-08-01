using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Facebook;
using Newtonsoft.Json.Linq;

namespace NietoYostenMvc.Code
{
    public class FacebookRegistrationInfo
    {
        public string Email;
        public string UserId;
        public string UserName;
    }

    public class FacebookUtil
    {
        private static readonly string appSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];

        public static string GetUserEmailTestValue { get; set; }

        static byte[] Base64UrlDecode(string s)
        {
            var decodedJson = s.Replace("=", string.Empty).Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(decodedJson.PadRight(decodedJson.Length + (4 - decodedJson.Length % 4) % 4, '='));
        }

        static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks that the signed request is valid
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static bool ValidateSignedRequest(string signed_request)
        {
            var split = signed_request.Split('.');
            string signature = split[0];
            string payload = split[1];

            var decodedSignature = Base64UrlDecode(signature);

            // Get expected hash
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
            var expectedSig = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            return ByteArraysEqual(decodedSignature, expectedSig);
        }

        /// <summary>
        /// Decode signed request sent by Facebook
        /// </summary>
        /// <param name="signedRequest">Signed request string</param>
        /// <returns>JObject containing decoded request</returns>
        public static JObject DecodeSignedRequest(string signedRequest)
        {
            string payload = signedRequest.Split('.')[1];
            var json = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            return JObject.Parse(json);
        }

        public static string GetAccessToken(string signedRequest)
        {
            var o = FacebookUtil.DecodeSignedRequest(signedRequest);
            return o.SelectToken("oauth_token").ToString().Replace("\"", "");
        }

        public static long GetFacebookUserId(string signedRequest)
        {
            var o = FacebookUtil.DecodeSignedRequest(signedRequest);
            return long.Parse(o.SelectToken("user_id").ToString().Replace("\"", ""));
        }

        public static string GetUserEmail(string accessToken)
        {
            if (FacebookUtil.GetUserEmailTestValue == null)
            {
                FacebookClient fbClient = new FacebookClient(accessToken);
                dynamic user = fbClient.Get("me");
                return user.email;
            }

            return FacebookUtil.GetUserEmailTestValue;
        }

        public static FacebookRegistrationInfo GetRegistrationInfo(string signedRequest)
        {
            // Decode signed_request sent by Facebook
            string payload = signedRequest.Split('.')[1];
            var json = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var o = JObject.Parse(json);
            string email = null;
            JToken regEmail = o.SelectToken("registration.email");

            if (regEmail != null)
            {
                email = regEmail.ToString().Replace("\"", "");
            }

            return new FacebookRegistrationInfo
            {
                UserId = o.SelectToken("user_id").ToString().Replace("\"", ""),
                Email = email
            };
        }
    }
}