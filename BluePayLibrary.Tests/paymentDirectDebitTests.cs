using System;
using System.ComponentModel;
using BluePayLibrary.Transaction;
using Xunit;

namespace BluePayLibrary.Tests
{
    [Trait("Category", "Integration")]
    [Collection("TestServerCollection")]
    public class PaymentDirectDebitTests
    {
        private string UserName = "";
        private string Password = "";
        private const string Mode = "TEST"; // run all tests in Test mode (use "LIVE" for production calls)
        private const string ResponseVersion = "4";
        private const string DefaultAccountType = "C"; //Account Type: Checking. Other option is "S" (savings). "C" is default
        private const string AchDocType = "WEB"; //ACH Document Type: WEB

        public Response ResponseParser;

        public PaymentDirectDebitTests()
        {
            UserName = TestHelper.TestUserName;
            Password = TestHelper.TestPassword;
        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayPaymentSepaAuthTest()
        {
            var testBic = "OKOYFIHH";
            var testIban = "DE89370400440532013000";

            var sepaPayment = new DirectDebitPayment
            {
                AccountType = "",
                Amount = 0,
                BankAccountNumber = testIban,
                CustomId = "A8G2LR6054V4C7G1WFP",
                DocType = AchDocType,
                RoutingNumber = testBic
            };

            var customerData = new CustomerFields()
            {
                Name1 = "Wonder",
                Name2 = "Woman",
            };

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);
            transaction.ProcessIntlDirectDebitAuth(sepaPayment, customerData);

            var result = transaction.GetRawResponse();
            Assert.NotNull(result);

            var response = new Response(result);
            Console.WriteLine("BluePayPaymentSepaAuthTest: " + response.ToString());

            //  Assert.Equal(customId, response.GetValue(ResponseFields.CustomId), "custom id");
            Assert.Equal(sepaPayment.Amount, Decimal.Parse(response.GetValue(ResponseFields.Amount)));
            Assert.Equal("SEPA", response.GetValue(ResponseFields.PaymentType));
            Assert.Equal(AchDocType, response.GetValue(RequestFields.DocType));
            Assert.Equal(customerData.Name1, response.GetValue(RequestFields.Name1));
            Assert.Equal(customerData.Name2, response.GetValue(RequestFields.Name2));

            Assert.Contains("APPROVED", response.GetValue(ResponseFields.Result));
            Assert.False(String.IsNullOrEmpty(response.GetValue(ResponseFields.ResultToken)), "token");

        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayPaymentSepaSaleTest()
        {
            var testBic = "OKOYFIHH";
            var testIban = "DE89370400440532013000";

            var sepaPayment = new DirectDebitPayment
            {
                AccountType = "",
                Amount = 3,
                BankAccountNumber = testIban,
                CustomId = "A8G2LR6054V4C7G1WFP",
                DocType = AchDocType,
                RoutingNumber = testBic
            };

            var customerData = new CustomerFields
            {
                Name1 = "James",
                Name2 = "Bond",
            };

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);
            transaction.ProcessIntlDirectDebitSale(sepaPayment, customerData);

            var result = transaction.GetRawResponse();
            Assert.NotNull(result);

            var response = new Response(result);

            Console.WriteLine("BluePayPaymentSepaSaleTest: " + response.ToString());


            //  Assert.Equal(customId, response.GetValue(ResponseFields.CustomId), "custom id");
            Assert.Equal(sepaPayment.Amount, Decimal.Parse(response.GetValue(ResponseFields.Amount)));
            Assert.Equal("SEPA", response.GetValue(ResponseFields.PaymentType));
            Assert.Equal(AchDocType, response.GetValue(RequestFields.DocType));
            Assert.Equal(customerData.Name1, response.GetValue(RequestFields.Name1));
            Assert.Equal(customerData.Name2, response.GetValue(RequestFields.Name2));

            Assert.Contains("APPROVED", response.GetValue(ResponseFields.Result));
            Assert.False(String.IsNullOrEmpty(response.GetValue(ResponseFields.ResultToken)), "token");

        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayPaymentDirectDebitSaleTest()
        {
            var customerData = new CustomerFields
            {
                Name1 = "Abe",
                Name2 = "Lincoln",
            };

            var directDebit = new DirectDebitPayment
            {
                AccountType = DefaultAccountType,
                Amount = 5,
                BankAccountNumber = "123456789",
                RoutingNumber = "123123123",
                DocType = AchDocType,
                CustomId = "ABC1234"
            };

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);
            transaction.ProcessUsDirectDebitSale(directDebit, customerData);

            var result = transaction.GetRawResponse();
            var response = new Response(result);

            Console.WriteLine("BluePayPaymentDirectDebitSaleTest: " + response.ToString());

            Assert.Equal(directDebit.Amount, Decimal.Parse(response.GetValue(ResponseFields.Amount)));
            Assert.Equal("ACH", response.GetValue(ResponseFields.PaymentType));
            Assert.Equal(AchDocType, response.GetValue(RequestFields.DocType));
            Assert.Equal(customerData.Name1, response.GetValue(RequestFields.Name1));
            Assert.Equal(customerData.Name2, response.GetValue(RequestFields.Name2));
           // Assert.Equal(directDebit.BankAccountNumber, response.GetValue(ResponseFields.PaymentAccount));
            Assert.Equal(directDebit.CustomId, response.GetValue(ResponseFields.CustomId));

            Assert.Contains("APPROVED", response.GetValue(ResponseFields.Result));
            Assert.False(String.IsNullOrEmpty(response.GetValue(ResponseFields.ResultToken)), "token");

        }

        [Fact]
        [Category("BluePayPayment")]
        public void BluePayPaymentDirectDebitAuthTest()
        {
            var customerData = new CustomerFields
            {
                Name1 = "Pablo",
                Name2 = "Picasso",
            };

            var routingNumber = "123123123";
            var bankAccountNumber = "123456789";
            var customId = "ABC1234";
            Decimal amount = 0; // Direct debit auth must be zero dollars or else an error is returned.

            var transaction = new Payment(UserName, Password, Mode, ResponseVersion);
            transaction.ProcessUsDirectDebitAuth(routingNumber, bankAccountNumber, DefaultAccountType, amount, customerData, AchDocType, customId);

            var result = transaction.GetRawResponse();
            var response = new Response(result);

            Console.WriteLine("BluePayPaymentDirectDebitAuthTest: " + response.ToString());

            Assert.Equal(amount, Decimal.Parse(response.GetValue(ResponseFields.Amount)));
            Assert.Equal("ACH", response.GetValue(ResponseFields.PaymentType));
            Assert.Equal(AchDocType, response.GetValue(RequestFields.DocType));
            Assert.Equal(customerData.Name1, response.GetValue(RequestFields.Name1));
            Assert.Equal(customerData.Name2, response.GetValue(RequestFields.Name2));
          //  Assert.Equal(bankAccountNumber, response.GetValue(ResponseFields.PaymentAccount));
            Assert.Equal(customId, response.GetValue(ResponseFields.CustomId));

            Assert.Contains("APPROVED", response.GetValue(ResponseFields.Result));
            Assert.False(String.IsNullOrEmpty(response.GetValue(ResponseFields.ResultToken)), "token");

        }

    }
}
