using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BluePayLibrary.Transaction;
using Xunit;

namespace BluePayLibrary.Tests
{
    [Trait("Category", "Integration")]
    [Collection("TestServerCollection")]
    public class PaymentCreditCardTests
    {
        private string UserName = "";
        private string Password = "";
        private string AuthToken = ""; 

        private const string Mode = "TEST"; // run all tests in Test mode (use "LIVE" for production calls)
        private const string ResponseVersion = "4";

        public Response ResponseParser;

        private CreditCardLevel3Defaults Level3Defaults;

        public PaymentCreditCardTests()
        {
            UserName = TestHelper.TestUserName;
            Password = TestHelper.TestPassword;

            Level3Defaults = new CreditCardLevel3Defaults
            {
                ItemCode = "CODE123",
                ItemDescription = "Level 3 Description",
                ProductCode = "HCS1",
                CommodityCode = "MCC7311",
                Quantity = "1",
                UnitOfMeasure = "EA"
            };
        }

        [Fact(DisplayName = "BluePayAuth")]
        [Category("BluePayPayment")]
        public void BluePayPaymentCreditCardAuthTest()
        {

            var customerData = new CustomerFields
            {
                Address1 = "123 Main Street",
                Address2 = "Suite 200",
                City = "St. Louis",
                Country = "USA",
                Email = "FakeEmail@test.com",
                Name1 = "George",
                Name2 = "Washington",
                Phone = "123-333-3333",
                State = "MO",
                ZipCode = "63102"
            };

            var transactionData = new CreditCardPayment
            {
                CardExpire = "1218",
                Cvv = "123",
                CardNumber = "4111111111111111"
            };

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);

            transaction.ProcessCreditCardAuth(transactionData, customerData, 3.00M);

            var result = transaction.GetRawResponse();

            var response = new Response(result);

            Console.WriteLine("BluePayPaymentCreditCardAuthTest: " + response.ToString() );

            Assert.NotNull(result);
            List<string> returnStrings = result.Split('&').ToList();

            Assert.True(returnStrings.Contains("TRANSACTION_TYPE=AUTH"), "AUTH");

            Assert.True(returnStrings.Contains("Result=APPROVED"), "approved");

            // store output so that token can be used for other tests
            AuthToken = response.GetValue(ResponseFields.ResultToken);
        }

 
        [Fact]
        [Category("BluePayPayment")]
        public void BluePayPaymentCreditCardTokenTest()
        {
            if (String.IsNullOrEmpty(AuthToken))
            {
                AuthToken = PerformCreditCardAuth();
            }

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);

            transaction.ProcessCreditCardSaleWithToken(3.00M, AuthToken);
            var result = transaction.GetRawResponse();
            var response = transaction.BluePayResponse;

            Console.WriteLine("BluePayPaymentCreditCardTokenTest: " + response.ToString());
            Assert.NotNull(result);

            Assert.Equal("3.00", response.GetValue(ResponseFields.Amount));
            Assert.Equal("CREDIT", response.GetValue(ResponseFields.PaymentType));
            Assert.Equal("SALE", response.GetValue(ResponseFields.TransactionType));
            Assert.Equal(AuthToken, response.GetValue(ResponseFields.PaymentToken));
            Assert.Equal("APPROVED", response.GetValue(ResponseFields.Result).ToUpper());
        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayCreditCardLevelThreeNoInvoiceTokenTest()
        {
            if (String.IsNullOrEmpty(AuthToken))
            {
                AuthToken = PerformCreditCardAuth();
            }
            Decimal productAmount = 7M;

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion, Level3Defaults);
            transaction.ProcessCreditCardSaleWithToken(productAmount, AuthToken);

            var result = transaction.GetRawResponse();
            var response = new Response(result);

            Console.WriteLine("BluePayCreditCardLevelThreeNoInvoiceTokenTest: " + response.ToString());

            //Assert.NotNull(result);
            //List<string> returnStrings = result.Split('&').ToList();
            //Assert.True(returnStrings.Contains("Result=APPROVED"), "approved");

            //Assert.False(returnStrings.Contains("LV3_ITEM1_TAX_RATE"), "no tax rate");
            //Assert.False(returnStrings.Contains("LV3_ITEM1_QUANTITY=1"), "no quantity");

        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayCreditCardLevelTwoAndThreeTokenTest()
        {
            if (String.IsNullOrEmpty(AuthToken))
            {
                AuthToken = PerformCreditCardAuth();
            }

            Decimal amount = 5.22M;
            var invoice = "CB11111";
            Decimal tax = .22M;

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion, Level3Defaults);

            transaction.ProcessCreditCardSaleWithToken(amount, AuthToken, invoice, tax);

            var result = transaction.GetRawResponse();

            var response = new Response(result);

            Console.WriteLine("BluePayCreditCardLevelTwoAndThreeTokenTest: " + response.ToString());


            Assert.NotNull(result);
            Assert.Equal("5.22", response.GetValue(ResponseFields.Amount));
            Assert.Equal("5.22", response.GetValue(ResponseFields.Level3LineItemTotal));
            Assert.Equal("APPROVED", response.GetValue(ResponseFields.Result).ToUpper());
            Assert.Equal(Level3Defaults.ItemDescription, response.GetValue(ResponseFields.Level3Description));
            Assert.Equal(Level3Defaults.CommodityCode, response.GetValue(ResponseFields.Level3CommodityCode));
            Assert.Equal("0.22", response.GetValue(ResponseFields.Level3ItemTax));
            Assert.Equal("0.22", response.GetValue(ResponseFields.Level3TaxRate));
            Assert.Equal("0.22", response.GetValue(ResponseFields.Tax));
            Assert.Equal("1", response.GetValue(ResponseFields.Level3Quantity));
            Assert.Equal(Level3Defaults.ProductCode, response.GetValue(ResponseFields.Level3ProductCode));
            Assert.Equal(Level3Defaults.UnitOfMeasure, response.GetValue(ResponseFields.Level3Units));
            Assert.Equal(invoice, response.GetValue(ResponseFields.InvoiceId));

        }

        /// <summary>
        /// Return an auth token that can be used for other tests
        /// </summary>
        /// <returns></returns>
        private string PerformCreditCardAuth()
        {
            var customerData = new CustomerFields
            {
                Address1 = "123 Main Street",
                Address2 = "Suite 200",
                City = "St. Louis",
                Country = "USA",
                Email = "FakeEmail@test.com",
                Name1 = "George",
                Name2 = "Washington",
                Phone = "123-333-3333",
                State = "MO",
                ZipCode = "63102"

            };

            var transactionData = new CreditCardPayment
            {
                CardExpire = "1218",
                Cvv = "123",
                CardNumber = "4111111111111111"
            };

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);
            transaction.ProcessCreditCardAuth(transactionData, customerData, 0);
            var result = transaction.GetRawResponse();
            var response = new Response(result);
            return  response.GetValue(ResponseFields.ResultToken);
        }

    }
}
