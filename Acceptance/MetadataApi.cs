using System;
using System.Net;
using Newtonsoft.Json;

namespace Acceptance
{
    public class LoginResponse
    {
        public string Token;
        public string Expires;
        public UserProfile UserProfile;

        public static LoginResponse CreateFromJson(string jsonRepresentation)
        {
            return JsonConvert.DeserializeObject<LoginResponse>(jsonRepresentation);
        }
    }

    public class UserProfile
    {
        public string FirstName;
        public string SubscriptionLevel;
        public bool AllowOfflineViewing;
        public OfflineViewingParameters OfflineViewingParameters;
    }

    public class OfflineViewingParameters
    {
        public int MaxDaysBeforeExpire;
        public int MaxModulesToCache;
    }

    public class MetadataApi
    {
        readonly string baseUrl;

        public MetadataApi(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        string BaseUrl
        {
            get { return baseUrl; }
        }

        public LoginResponse Login(string userName, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var url = LoginUrl(userName);
            var payload = string.Format("password={0}", password);
            var httpWebResponse = HttpHelpers.HttpPost(url, payload, "application/x-www-form-urlencoded");
            var responseBody = HttpHelpers.GetResponseBody(httpWebResponse);
            return LoginResponse.CreateFromJson(responseBody);
        }

        string LoginUrl(string userName)
        {
            return string.Format("{0}/users/{1}/login", ToHttps(BaseUrl), userName);
        }

        static string ToHttps(string url)
        {
            return url.Replace("http", "https");
        }
    }
}