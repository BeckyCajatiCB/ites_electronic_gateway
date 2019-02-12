using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePayLibrary
{
    /// <summary>
    /// These are the fields found in the Blue Pay response
    /// </summary>
    public static class RequestFields
    {
        public static string AchRoutingNumber = "ACH_ROUTING";

        public static string AchAccountNumber = "ACH_ACCOUNT";

        public static string AchAccountType = "ACH_ACCOUNT_TYPE";

        public static string AuthCode = "AUTH_CODE";

        public static string Bic = "BIC";

        public static string Iban = "IBAN";

        public static string DocType = "DOC_TYPE";

        public static string PaymentAccount = "PAYMENT_ACCOUNT";

        public static string PaymentType = "PAYMENT_TYPE";

        public static string Rebid = "REBID";

        public static string Merchant = "MERCHANT";

        public static string Name1 = "NAME1";

        public static string Name2 = "NAME2";


        public static string TransactionType = "TRANSACTION_TYPE";

        public static string CustomId = "CUSTOM_ID";

        public static string CustomId2 = "CUSTOM_ID2";

        public static string ResponseVersion = "RESPONSEVERSION";

        // Any strings that start with "MERCHDATA_" that are sent to Blue Pay will be returned also
        // Add those here...

        public static string TransactionId = "MERCHDATA_TRANSACTIONID";


    }


    public static class ResponseFields
    {
        public static string Amount = "AMOUNT";

        public static string AuthCode = "AUTH_CODE";

        public static string Avs = "AVS";

        public static string BankName = "BANK_NAME";

        public static string CardExpire = "CARD_EXPIRE";

        public static string CardType = "CARD_TYPE";

        public static string CVV2 = "CVV2";

        public static string InvoiceId = "INVOICE_ID";

        public static string Message = "MESSAGE";

        public static string OrderId = "ORDER_ID";

        public static string PaymentAccount = "PAYMENT_ACCOUNT";

        public static string PaymentType = "PAYMENT_TYPE";

        public static string Rebid = "REBID";

        public static string Result = "Result";

        /// <summary>
        /// Token used as authorization for the transaction
        /// </summary>
        public static string PaymentToken = "MASTER_ID";

        /// <summary>
        /// Result token returned from the transaction
        /// </summary>
        public static string ResultToken = "RRNO";

        public static string Missing = "MISSING";

        public static string Merchant = "MERCHANT";

        public static string TransactionType = "TRANSACTION_TYPE";

        public static string CustomId = "CUSTOM_ID";

        public static string CustomId2 = "CUSTOM_ID2";

        public static string Tax = "AMOUNT_TAX";

        public static string Level3Description = "LV3_ITEM1_ITEM_DESCRIPTOR";

        public static string Level3CommodityCode = "LV3_ITEM1_COMMODITY_CODE";

        public static string Level3TaxRate = "LV3_ITEM1_TAX_RATE";

        public static string Level3Quantity = "LV3_ITEM1_QUANTITY";

        public static string Level3ProductCode = "LV3_ITEM1_PRODUCT_CODE";

        public static string Level3Units = "LV3_ITEM1_MEASURE_UNITS";

        public static string Level3ItemTax = "LV3_ITEM1_TAX_AMOUNT";

        public static string Level3ItemCost = "LV3_ITEM1_UNIT_COST";

        public static string Level3LineItemTotal = "LV3_ITEM1_LINE_ITEM_TOTAL";

    }

}
