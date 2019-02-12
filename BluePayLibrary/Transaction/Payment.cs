using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BluePayLibrary.Post;

namespace BluePayLibrary.Transaction
{
    /// <summary>
    ///  This class represents the data and interfaces for a single Blue Pay transaction.
    ///  This library supports the following transactions:
    ///     * Direct Debit Auth (US and SEPA)
    ///     * Direct Debit Sale (US and SEPA)
    ///     * Sales, Refund, Void using a charging token
    /// </summary>
    public class Payment
    {

        // This is the only API supported by this version of the Blue Pay library
        private string transactionURL = "https://secure.bluepay.com/interfaces/bp10emu";

        public Response BluePayResponse { get; set; }

        // Note: Blue Pay documentations says MandateID and MandateData are required,
        //       But we are nto supplying these.
        public string MandateId = ""; // required for SEPA, merchant supplied Mandate ID
        public string MandateDate = ""; // required for SEPA 'YYYY-MM-DD'

        private readonly string _accountId = "";

        private readonly string _mode = "";

        private readonly string _responseVersion = "3";

        private readonly string _secretKey = "";

        private readonly BluePayPost _bluePayPost;

        private readonly CreditCardLevel3Defaults _level3DefaultValues;


        public Payment(string accountId, string secretKey, string mode, string version = "3", 
                        CreditCardLevel3Defaults level3Defaults = new CreditCardLevel3Defaults())
        {
            _accountId = accountId;
            _secretKey = secretKey;
            _mode = mode;
            _responseVersion = version;
            _bluePayPost = new BluePayPost(transactionURL);
            _level3DefaultValues = level3Defaults;

            // when a new transaction is processed, the latest raw response creates a new response library
            BluePayResponse = new Response(); 

        }

        /// <summary>
        ///     Runs an Auth Transaction
        /// </summary>
        public void ProcessUsDirectDebitAuth(string routingNumber, string bankAccountNumber, string accountType, decimal amount,
                                             CustomerFields customerData, string docType = "", string customId = "")
        {
            var transType = "AUTH";
            var paymentType = "ACH";

            var paymentData = String.Format("&PAYMENT_TYPE={0}&ACH_ROUTING={1}&ACH_ACCOUNT={2}&ACH_ACCOUNT_TYPE={3}&DOC_TYPE={4}&NAME1={5}&NAME2={6}&CUSTOM_ID={7}",
                                             HttpUtility.UrlEncode(paymentType), HttpUtility.UrlEncode(routingNumber),
                                             HttpUtility.UrlEncode(bankAccountNumber), HttpUtility.UrlEncode(accountType),
                                             HttpUtility.UrlEncode(docType), HttpUtility.UrlEncode(customerData.Name1), HttpUtility.UrlEncode(customerData.Name2),
                                             HttpUtility.UrlEncode(customId));

            ProcessPayment(paymentData, transType, amount, "");
        }

        /// <summary>
        ///     ProcessUsDirectDebitSale
        /// </summary>
        /// <param name="directDebit">Details for this Direct Debit Transaction</param>
        /// <param name="customerData">Customer Details</param>
        public void ProcessUsDirectDebitSale(DirectDebitPayment directDebit, CustomerFields customerData)
        {
            var paymentType = "ACH";
            var transType = "SALE";

            var paymentData =
                    String.Format("&PAYMENT_TYPE={0}&ACH_ROUTING={1}&ACH_ACCOUNT={2}&ACH_ACCOUNT_TYPE={3}&DOC_TYPE={4}&NAME1={5}&NAME2={6}&CUSTOM_ID={7}",
                    HttpUtility.UrlEncode(paymentType),
                    HttpUtility.UrlEncode(directDebit.RoutingNumber), HttpUtility.UrlEncode(directDebit.BankAccountNumber),
                    HttpUtility.UrlEncode(directDebit.AccountType),HttpUtility.UrlEncode(directDebit.DocType),
                    HttpUtility.UrlEncode(customerData.Name1), HttpUtility.UrlEncode(customerData.Name2), HttpUtility.UrlEncode(directDebit.CustomId));

            ProcessPayment(paymentData, transType, directDebit.Amount, "");
        }

        /// <summary>
        ///     Runs an Auth Transaction
        /// </summary>
        public void ProcessIntlDirectDebitAuth(DirectDebitPayment directDebit,CustomerFields customerData)
        {
            var transType = "AUTH";

            ProcessSepaTransaction(directDebit, customerData,transType);
        }

        /// <summary>
        ///     Runs an Auth Transaction
        /// </summary>
        public void ProcessIntlDirectDebitSale(DirectDebitPayment sepaPayment, CustomerFields customerData)
        {
            var transType = "SALE";

            ProcessSepaTransaction(sepaPayment, customerData, transType);
        }

        private void ProcessSepaTransaction(DirectDebitPayment sepaPayment, CustomerFields customerData,  string transType)
        {
            var paymentType = "SEPA";

            // to do - documentation says we need MANDATE_ID and MANDATE_DATE also
            var paymentData =
                String.Format(
                    "&BIC={0}&IBAN={1}&DOC_TYPE={2}&PAYMENT_TYPE={3}&NAME1={4}&NAME2={5}&ACH_ACCOUNT_TYPE={6}&CUSTOM_ID={7}",
                    HttpUtility.UrlEncode(sepaPayment.RoutingNumber),
                    HttpUtility.UrlEncode(sepaPayment.BankAccountNumber), HttpUtility.UrlEncode(sepaPayment.DocType), HttpUtility.UrlEncode(paymentType),
                    HttpUtility.UrlEncode(customerData.Name1), HttpUtility.UrlEncode(customerData.Name2), HttpUtility.UrlEncode(sepaPayment.AccountType),
                    HttpUtility.UrlEncode(sepaPayment.CustomId));

            ProcessPayment(paymentData, transType, sepaPayment.Amount, "");
        }

        /// <summary>
        ///     Runs a Sale Transaction
        /// </summary>
        public void ProcessCreditCardAuth(CreditCardPayment cardData, CustomerFields customerData, Decimal amount)
        {
            var transType = "AUTH";

            string paymentData = string.Format("&PAYMENT_TYPE=CREDIT&CC_NUM={0}&CC_EXPIRES={1}&CVCVV2={2}&NAME1={3}&NAME2={4}&ADDR1={5}&ADDR2={6}&CITY={7}&STATE={8}&ZIPCODE={9}",
                cardData.CardNumber,
                cardData.CardExpire,
                cardData.Cvv,
                customerData.Name1,
                customerData.Name2,
                customerData.Address1,
                customerData.Address2,
                customerData.City,
                customerData.State,
                customerData.ZipCode);

            ProcessPayment(paymentData, transType, amount, "");

        }

        public void ProcessCreditCardSaleWithToken(Decimal amount, string token, string invoice = "", decimal tax = 0)
        {
            ProcessSaleWithToken("CREDIT", amount, token, invoice, tax);
        }

        private void ProcessSaleWithToken(string paymentType, Decimal amount, string token, string invoice, decimal tax)
        {
            var transactionData = new TransactionFields(amount, token, "SALE", paymentType, _level3DefaultValues, invoice, tax);

            string paymentData =
                $"&RRNO={HttpUtility.UrlEncode(token)}&PAYMENT_TYPE={HttpUtility.UrlEncode(transactionData.PaymentType)}";
       
            ProcessPayment(paymentData, "SALE", amount, token);
        }


        /// <summary>
        ///     Runs a Refund Transaction
        /// </summary>
        public void ProcessRefund(string masterId, decimal amount)
        {
            var transType = "REFUND";

            string paymentData = string.Format("&RRNO={0}", HttpUtility.UrlEncode(masterId));

            ProcessPayment(paymentData, transType, amount, masterId);
        }

        /// <summary>
        ///     Voids a transaction
        /// </summary>
        /// <param name="token used for the sale that is being voided"></param>
        public void ProcessVoid(string masterId)
        {
            var transType = "VOID";

            string paymentData = string.Format("&RRNO={0}", HttpUtility.UrlEncode(masterId));

            ProcessPayment(paymentData, transType, null, masterId);
        }


        /// <summary>
        ///     Calculates TAMPER_PROOF_SEAL 
        /// </summary>
        private string CalcTps(string secretKey, string accountId, string transType, string amountText, string masterId, string mode)
        {
            string tamperProofSeal = String.Format("{0}{1}{2}{3}{4}{5}", secretKey, accountId,
                transType, amountText, masterId, _mode);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            ASCIIEncoding encode = new ASCIIEncoding();

            byte[] buffer = encode.GetBytes(tamperProofSeal);
            hash = md5.ComputeHash(buffer);
            return ByteArrayToString(hash);
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

        private string ProcessPayment(string paymentData, string transType, decimal? amount, string masterId)
        {
            var amountText = amount == null ? "" : amount.ToString();
            var tps = CalcTps(_secretKey, _accountId, transType, amountText, masterId, _mode);

            string postData =
                String.Format("MERCHANT={0}&MODE={1}&TRANSACTION_TYPE={2}&AMOUNT={3}&TAMPER_PROOF_SEAL={4}&REMOTE_IP={5}&RESPONSEVERSION={6}",
                    HttpUtility.UrlEncode(_accountId), HttpUtility.UrlEncode(_mode),
                    HttpUtility.UrlEncode(transType), HttpUtility.UrlEncode(amountText), HttpUtility.UrlEncode(tps),
                    Dns.GetHostEntry("").AddressList[0], _responseVersion);


            // concatenate the general information (postData) with the specific payment information (paymentData)
            postData = String.Format("{0}{1}", postData, paymentData);

            //await BluePayPostAsync.PerformAsyncPost(postData, transactionURL);
            _bluePayPost.PerformPost(postData);

            BluePayResponse = new Response(_bluePayPost.GetRawResponse());

            return _bluePayPost.GetStatus();
        }

        /// <summary>
        /// Add fields required for Level 3 Processing
        /// </summary>
        /// <param name="lineItemFields">Data required for Level 3 processing</param>
        /// <param name="paymentFields">Data required for Level 2 processing</param>
        /// <returns></returns>
        private string LevelTwoAndThreeDataFields(LevelThreeFields lineItemFields, LevelTwoFields paymentFields)
        {
            // we are assuming the we either have all or none of level 2 & 3 data. If no invoice #, then don't include these fields
            if (String.IsNullOrWhiteSpace(paymentFields.Invoice))
                return "";

            var levelTwoData = $"&INVOICE_ID={paymentFields.Invoice}&AMOUNT_TAX={paymentFields.Tax}";
           
            var levelThreeData =
                $"&LV3_ITEM1_PRODUCT_CODE={HttpUtility.UrlEncode(lineItemFields.ProductCode)}" +
                $"&LV3_ITEM1_UNIT_COST={HttpUtility.UrlEncode(lineItemFields.UnitCost)}" +
                $"&LV3_ITEM1_QUANTITY={HttpUtility.UrlEncode(lineItemFields.Quantity)}" +
                $"&LV3_ITEM1_ITEM_DESCRIPTOR={HttpUtility.UrlEncode(lineItemFields.ItemDescription)}" +
                $"&LV3_ITEM1_MEASURE_UNITS={HttpUtility.UrlEncode(lineItemFields.UnitOfMeasure)}" +
                $"&LV3_ITEM1_COMMODITY_CODE={HttpUtility.UrlEncode(lineItemFields.CommodityCode)}" +
                $"&LV3_ITEM1_TAX_AMOUNT={HttpUtility.UrlEncode(lineItemFields.TaxAmount)}" +
                $"&LV3_ITEM1_TAX_RATE={HttpUtility.UrlEncode(lineItemFields.TaxRate)}" +
                $"&LV3_ITEM1_LINE_ITEM_TOTAL={HttpUtility.UrlEncode(lineItemFields.LineItemTotal)}";

            return $"{levelTwoData}{levelThreeData}";
        }


        // Added this function to return the  response from the last processed message
        // using this for testing
        public string GetRawResponse()
        {
            return _bluePayPost.GetRawResponse();
        }

    }

}
