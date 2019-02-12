using System.Text;
using BluePayLibrary.Post;

namespace BluePayLibrary
{
    using System.Security.Cryptography;
    using System.Web;

    public class Query
    {

        private string queryUrl = "https://secure.bluepay.com/interfaces/stq";

        private BluePayPost bluePayPost;


        public string TPS = "";

        private decimal _amount;

        public string transType = "";

        public string accountID = "";

        private string amountText = "";

        public string doNotEscape = "";

        public string docType = "";

        public string excludeErrors = "";

        public string masterID = "";

        //public string memo = "";

        public string mode = "";

        public string name1 = "";

        public string name2 = "";



        public string paymentAccount = "";

        public string paymentType = "";


        public string queryByHierarchy = "";

        public string queryBySettlement = "";


        public string reportEndDate = "";

        public string reportStartDate = "";

        public string response = "";

        //// Added this. Not sure why it is not in Blue Pays existing lib
        public string responseVersion = "3";

        //public string routingNum = "";

        public string secretKey = "";

        //public string templateID = "";


        public Query(string accountID, string secretKey, string mode, string version)
        {
            this.accountID = accountID;
            this.secretKey = secretKey;
            this.mode = mode;
            this.responseVersion = version;
            bluePayPost = new BluePayPost(queryUrl);
        }

        public Query(string accountID, string secretKey, string mode)
            : this(accountID, secretKey, mode, "3")
        {
        }

        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                amountText = _amount.ToString();
            }
        }



        /// <summary>
        ///     Gets Details of a Transaction
        /// </summary>
        /// <param name="reportStart"></param>
        /// <param name="reportEnd"></param>
        /// <param name="errors"></param>
        public void GetSingleTransQuery(string transactionID, string reportStartDate, string reportEndDate, string errors = null)
        {
            this.reportStartDate = reportStartDate;
            this.reportEndDate = reportEndDate;
            this.excludeErrors = errors;
            this.masterID = transactionID;
        }

        /// <summary>
        ///     Queries by Transaction ID. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="transID"></param>
        public void QueryByTransactionID(string transID)
        {
            this.masterID = transID;
        }

        /// <summary>
        ///     Queries by Payment Type. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="payType"></param>
        public void QueryByPaymentType(string payType)
        {
            this.paymentType = payType;
        }

        /// <summary>
        ///     Queries by Transaction Type. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="transType"></param>
        public void QueryBytransType(string transType)
        {
            this.transType = transType;
        }

        /// <summary>
        ///     Queries by Transaction Amount. To be used with getSingleTransQuery
        /// </summary>
        /// <param name="amount"></param>
        public void QueryByAmount(decimal amount)
        {
            Amount = amount;
        }

        /// <summary>
        ///     Queries by First Name (NAME1) . To be used with getSingleTransQuery
        /// </summary>
        /// <param name="name1"></param>
        public void QueryByName1(string name1)
        {
            this.name1 = name1;
        }

        /// <summary>
        ///     Queries by Last Name (NAME2) . To be used with getSingleTransQuery
        /// </summary>
        /// <param name="name2"></param>
        public void QueryByName2(string name2)
        {
            this.name2 = name2;
        }


        /// <summary>
        ///     Calculates TAMPER_PROOF_SEAL for bpdailyreport2 and stq APIs
        /// </summary>
        public void CalcReportTPS()
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

                    CalcReportTPS();
                    postData += "ACCOUNT_ID=" + HttpUtility.UrlEncode(this.accountID) + "&MODE=" + HttpUtility.UrlEncode(this.mode) + "&TAMPER_PROOF_SEAL="
                                + HttpUtility.UrlEncode(this.TPS) + "&REPORT_START_DATE=" + HttpUtility.UrlEncode(this.reportStartDate) + "&REPORT_END_DATE="
                                + HttpUtility.UrlEncode(this.reportEndDate) + "&EXCLUDE_ERRORS=" + HttpUtility.UrlEncode(this.excludeErrors);
                    postData += (this.masterID != "") ? "&id=" + HttpUtility.UrlEncode(this.masterID) : "";
                    postData += (this.paymentType != "") ? "&payment_type=" + HttpUtility.UrlEncode(this.paymentType) : "";
                    postData += (this.transType != "") ? "&trans_type=" + HttpUtility.UrlEncode(this.transType) : "";
                    postData += (amountText != "") ? "&amount=" + HttpUtility.UrlEncode(amountText) : "";
                    postData += (this.name1 != "") ? "&name1=" + HttpUtility.UrlEncode(this.name1) : "";
                    postData += (this.name2 != "") ? "&name2=" + HttpUtility.UrlEncode(this.name2) : "";

            bluePayPost.PerformPost(postData);

            return bluePayPost.GetStatus();
        }


        // Added this function to return the  response from the last processed message
        // using this for testing
        public string GetRawResponse()
        {
            return bluePayPost.GetRawResponse();
        }

    }

}
