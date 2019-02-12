using System;

namespace BluePayLibrary
{
    public class DirectDebitPayment
    {
        /// <summary>
        /// Routing number for US
        /// BIC for SEPA (intl) payments
        /// </summary>
        public string RoutingNumber { get; set; }

        /// <summary>
        /// Bank account for US
        /// IBAN for SEPA (intl) payments
        /// </summary>
        public string BankAccountNumber { get; set; }

        public string CustomId { get; set; }

        public string AccountType { get; set; }

        /// <summary>
        /// Optional Field. We are setting it to: 
        ///  'WEB': Indicates the customer has agreed to the charges via an internet-based or 
        /// electronic form.
        /// </summary> 
        public string DocType { get; set; }

        public Decimal Amount { get; set; }

    }
}
