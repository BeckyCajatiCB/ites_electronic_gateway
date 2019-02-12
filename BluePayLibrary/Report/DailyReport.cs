using System.Security.Cryptography;
using System.Text;
using System.Web;
using BluePayLibrary.Post;

namespace BluePayLibrary.Report
{
    public class DailyReport
    {
        private string reportUrl = "https://secure.bluepay.com/interfaces/bpdailyreport2";

        private BluePayPost bluePayCommunication;

        public string TPS = "";

        public string accountID = "";

        public string doNotEscape = "";

        public string excludeErrors = "";

        public readonly string mode = "";

        public string queryByHierarchy = "";

        public string queryBySettlement = "";

        public string reportEndDate = "";

        public string reportStartDate = "";

        public string response = "";

        private string responseVersion = "4";

        private string secretKey = "";


        public DailyReport(string accountID, string secretKey, string mode, string version)
        {
            this.accountID = accountID;
            this.secretKey = secretKey;
            this.mode = mode;
            this.responseVersion = version;
            bluePayCommunication = new BluePayPost(reportUrl);
        }

        public DailyReport(string accountID, string secretKey, string mode)
            : this(accountID, secretKey, mode, "4")
        {
        }
   

        /// <summary>
        ///     Gets Report of Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        /// <param name="errors"></param>
        public void GetTransactionReport(
            string reportStartDate,
            string reportEndDate,
            string subaccountsSearched,
            string doNotEscape = null,
            string excludeErrors = null)
        {
            this.queryBySettlement = "0";
            this.reportStartDate = reportStartDate;
            this.reportEndDate = reportEndDate;
            this.queryByHierarchy = subaccountsSearched;
            this.doNotEscape = doNotEscape;
            this.excludeErrors = excludeErrors;
        }

        /// <summary>
        ///     Gets Report of Settled Transaction Data
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="subaccountsSearched"></param>
        /// <param name="doNotEscape"></param>
        /// <param name="errors"></param>
        public void GetTransactionSettledReport(
            string reportStartDate,
            string reportEndDate,
            string subaccountsSearched,
            string doNotEscape = null,
            string excludeErrors = null)
        {
            this.queryBySettlement = "1";
            this.reportStartDate = reportStartDate;
            this.reportEndDate = reportEndDate;
            this.queryByHierarchy = subaccountsSearched;
            this.doNotEscape = doNotEscape;
            this.excludeErrors = excludeErrors;
        }

        /// <summary>
        ///     Calculates TAMPER_PROOF_SEAL for bpdailyreport2 and stq APIs
        /// </summary>
        private void CalcReportTPS()
        {
            string tamper_proof_seal = this.secretKey + this.accountID + this.reportStartDate + this.reportEndDate;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamper_proof_seal);
            hash = md5.ComputeHash(buffer);
            this.TPS = ByteArrayToString(hash);
        }


        //This is used to convert a byte array to a hex string
        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        public string Process()
        {
            string postData = "";

            queryBySettlement = "0";
            queryByHierarchy = "0";
            excludeErrors = "1";
            doNotEscape = "0";


            CalcReportTPS();
            postData += "ACCOUNT_ID=" + HttpUtility.UrlEncode(this.accountID) + "&MODE=" + HttpUtility.UrlEncode(this.mode) + "&TAMPER_PROOF_SEAL="
                        + HttpUtility.UrlEncode(this.TPS) + "&REPORT_START_DATE=" + HttpUtility.UrlEncode(this.reportStartDate) + "&REPORT_END_DATE="
                        + HttpUtility.UrlEncode(this.reportEndDate) + "&DO_NOT_ESCAPE=" + HttpUtility.UrlEncode(this.doNotEscape)
                        + "&QUERY_BY_SETTLEMENT=" + HttpUtility.UrlEncode(this.queryBySettlement) + "&QUERY_BY_HIERARCHY="
                        + HttpUtility.UrlEncode(this.queryByHierarchy) + "&EXCLUDE_ERRORS=" + HttpUtility.UrlEncode(this.excludeErrors)
                        + "&RESPONSE_VERSION=" + HttpUtility.UrlEncode(this.responseVersion);

            bluePayCommunication.PerformPost(postData);

            return bluePayCommunication.GetStatus();
        }

    }


}
