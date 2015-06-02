using System;
using System.IO;
using System.Net;
using System.Text;

namespace Acceptance
{
    public static class HttpHelpers
    {
        public static HttpWebResponse HttpPost(string url, string payload, string contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(payload);

            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = data.Length;

            using (var newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }

            return (HttpWebResponse)request.GetResponse();
        }

        public static HttpWebResponse HttpGet(string url, int timeoutInSeconds = 100)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeoutInSeconds * 1000;
            return (HttpWebResponse)request.GetResponse();
        }

        public static string GetResponseBody(HttpWebResponse httpWebResponse)
        {
            var responseStream = httpWebResponse.GetResponseStream();
            var responseBody = "";
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    responseBody = reader.ReadToEnd();
                }
            }
            return responseBody;
        }

        public static string UriEncode(string searchString)
        {
            return Uri.EscapeDataString(searchString);
        }
    }
}