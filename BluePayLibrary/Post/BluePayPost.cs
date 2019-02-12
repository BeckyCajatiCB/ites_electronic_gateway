using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BluePayLibrary.Post
{
    public class BluePayPost
    {
        private int numRetries = 0;

        private static string rawResponse = "";

        private string response = "";

        private static string Url;

        private static readonly HttpClient client = new HttpClient();

        public BluePayPost(string url)
        {
            Url = url;
        }


        public void PerformPost(string post)
        {
            //Create HTTPS POST object and send to BluePay
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Url));
          //  request.AllowAutoRedirect = false;

            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            byte[] data = encoding.GetBytes(post);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                Stream postdata = request.GetRequestStream();

                postdata.Write(data, 0, data.Length);
                postdata.Close();

                // Get response

                GetResponse(request);
            }
            catch (WebException e)
            {
                if (e.Message.Contains("302"))
                {
                    var webResponse = e.Response;
                  //  rawResponse = webResponse.("Location");

                }

            }
            //catch (WebException e)
            //{
            //    HttpWebResponse httpResponse = (HttpWebResponse)e.Response;
            //    try
            //    {
            //        GetResponse(e, post);
            //        httpResponse.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("An error has occurred while connecting to BluePay. Retrying connection...");
            //    }
            //}
        }

        // Added this function to return the  response from the last processed message
        // using this for testing
        public string GetRawResponse()
        {
            return rawResponse;
        }

        public void GetResponse(HttpWebRequest request)
        {
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();

            rawResponse = httpResponse.GetResponseHeader("Location");

            httpResponse.Close();
        }

        public void GetResponse(WebException request, string postData)
        {
            // Due to how Microsoft handles SSL/TLS session caching, a retry on a failed connection with an expired set of keys may be necessary.
            if (numRetries < 1 && request.Message == "The request was aborted: Could not create SSL/TLS secure channel.")
            {
                numRetries++;
                PerformPost(postData);
            }
        }

        /// <summary>
        ///     Returns STATUS or status from response
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            Regex r = new Regex(@"Result=([^&$]*)");
            Match m = r.Match(response);
            if (m.Success)
            {
                return (m.Value.Substring(7));
            }
            r = new Regex(@"status=([^&$]*)");
            m = r.Match(response);
            if (m.Success)
            {
                return (m.Value.Substring(7));
            }
            return "";
        }
    }
}
