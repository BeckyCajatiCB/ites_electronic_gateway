using System;
using System.Globalization;

namespace BluePayLibrary.Transaction
{
    public class TransactionFields
    {

        public TransactionFields(Decimal? amount, string token, 
                                 string transaction, string paymentType,
                                 CreditCardLevel3Defaults level3Defaults,
                                 string invoice, Decimal? tax)
        {
            AmountText = amount == null ? "" : amount.ToString();
            string taxAmount = tax == null ? "" : tax.ToString();

            PaymentToken = token;
            TransactionType = transaction;
            PaymentType = paymentType;

            PaymentDetails = new LevelTwoFields
            {
                Tax = taxAmount,
                Invoice = invoice
            };

            LineItem = new LevelThreeFields
            {
                ItemCode = level3Defaults.ItemCode,
                ItemDescription = level3Defaults.ItemDescription,
                LineItemTotal = AmountText,
                ProductCode = level3Defaults.ProductCode,
                CommodityCode = level3Defaults.CommodityCode,
                Quantity = level3Defaults.Quantity,
                TaxAmount = taxAmount,
                TaxRate = taxAmount,
                UnitCost = AmountText,
                UnitOfMeasure = level3Defaults.UnitOfMeasure
            };
        }
        public string TransactionType { get; set; }

        /// <summary>
        /// Values: CREDIT, ACH, SEPA, DD 
        /// </summary>
        public string PaymentType { get; set; }

        public string AmountText { get; set; }

        public string PaymentToken { get; set; }

        public LevelTwoFields PaymentDetails { get; set; }

        public LevelThreeFields LineItem { get; set; }
  
    }

    public class LevelTwoFields
    {
        public string Invoice { get; set; }

        public string Tax { get; set; }
    }

    public class LevelThreeFields
    {
        public string ItemDescription { get; set; }

        public string ItemCode { get; set; }

        public string ProductCode { get; set; }

        public string CommodityCode { get; set; }

        public string UnitOfMeasure { get; set; }

        public string UnitCost { get; set; }

        public string Quantity { get; set; }

        public string TaxRate { get; set; }

        public string TaxAmount { get; set; }

        public string LineItemTotal { get; set; }

    }

    public class LevelThreeCanadaFields
    {
        public string ProductSku { get; set; }

        public string ItemDiscount { get; set; }
    }
}
