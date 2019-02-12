using System.Text;

namespace BluePayLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    ///     This class provides methods for getting specific data out of the
    ///     Blue Pay Response
    /// </summary>
    public class Response
    {
        private readonly string response = "";

        private  Dictionary<string, string> ResponseValues = new Dictionary<string, string>();

        public Response()
        {
            
        }

        public Response(string rawResponse)
        {
            // remove the initial string up to the ? of the response
            int index = rawResponse.IndexOf('?');
            response = rawResponse.Substring(index+1);

            ResponseValues = new Dictionary<string, string>();
            var nameValueArray = response.Split('&');

            nameValueArray.ToList().ForEach(s =>
            {
                if (!s.Contains("="))
                {
                    ResponseValues.Add(s.ToUpper(), string.Empty);
                }
                else
                {
                    var nameValueSplit = s.Split('=');
                    var key = HttpUtility.UrlDecode(nameValueSplit[0].ToUpper());
                    if (!ResponseValues.ContainsKey(key))
                    {
                        ResponseValues.Add(key.ToUpper().Trim(), HttpUtility.UrlDecode(nameValueSplit[1]));
                    }
                }
            });
        }

        public string GetValue(string key)
        {
            return ResponseValues.ContainsKey(key.ToUpper()) ? ResponseValues[key.ToUpper()] : String.Empty;
        }


        /// <summary>
        ///     Returns MESSAGE from Response
        /// </summary>"
        /// <returns></returns>
        public string GetMessage()
        {
            var r = new Regex(@"MESSAGE=([^&$]+)");
            var m = r.Match(response);
            if (m.Success)
            {
                var message = m.Value.Substring(8).Split('"');
                return message[0];
            }
            return "";
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            foreach (var key in ResponseValues.Keys)
            {
                output.Append(key);
                output.Append(":");
                output.Append(ResponseValues[key]);
                output.Append("||");
            }

            return output.ToString();
        }

        ///// <summary>
        /////     Returns CVV2 from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetCVV2()
        //{
        //    var r = new Regex(@"CVV2=([^&$]*)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(5);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns AVS from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetAVS()
        //{
        //    var r = new Regex(@"AVS=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(4);
        //    }
        //    return "";
        //}

        /// <summary>
        ///     Returns Token (RRNO) from Response
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            var r = new Regex(@"RRNO=([^&$]+)");
            var m = r.Match(response);
            if (m.Success)
            {
                return m.Value.Substring(12);
            }
            return "";
        }

        ///// <summary>
        /////     Returns PAYMENT_ACCOUNT from response
        ///// </summary>
        ///// <returns></returns>
        //public string GetMaskedPaymentAccount()
        //{
        //    var r = new Regex("PAYMENT_ACCOUNT=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(16);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns CARD_TYPE from response
        ///// </summary>
        ///// <returns></returns>
        //public string GetCardType()
        //{
        //    var r = new Regex("CARD_TYPE=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(10);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns BANK_NAME from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetBank()
        //{
        //    var r = new Regex("BANK_NAME=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(10);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns AUTH_CODE from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetAuthCode()
        //{
        //    var r = new Regex(@"AUTH_CODE=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(10);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns creation_date from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetCreationDate()
        //{
        //    var r = new Regex(@"creation_date=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(14);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns next_date from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetNextDate()
        //{
        //    var r = new Regex(@"next_date=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(10);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns last_date from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetLastDate()
        //{
        //    var r = new Regex(@"last_date=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(9);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns sched_expr from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetSchedExpr()
        //{
        //    var r = new Regex(@"sched_expr=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(11);
        //    }
        //    return "";
        //}

        ///// <summary>
        /////     Returns cycles_remain from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetCyclesRemain()
        //{
        //    var r = new Regex(@"cycles_remain=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(14);
        //    }
        //    return "";
        //}


        ///// <summary>
        /////     Returns next_amount from Response
        ///// </summary>
        ///// <returns></returns>
        //public string GetNextAmount()
        //{
        //    var r = new Regex(@"next_amount=([^&$]+)");
        //    var m = r.Match(response);
        //    if (m.Success)
        //    {
        //        return m.Value.Substring(12);
        //    }
        //    return "";
        //}
    }
}