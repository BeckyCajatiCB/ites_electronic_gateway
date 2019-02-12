using Xunit;

namespace BluePayLibrary.Tests
{
    [Trait("Category", "Integration")]
    [Collection("TestServerCollection")]
    public class QueryTest
    {
        private string UserName = "";
        private string Password = "";

        private const string Mode = "TEST"; // run all tests in Test mode (use "LIVE" for production calls)

        private const string ResponseVersion = "4";
        //private const string AccountType = "C"; //Account Type: Checking. Other option is "S" (savings). "C" is default
        //private const string AchDocType = "WEB"; //ACH Document Type: WEB

        public  QueryTest()
        {
            UserName = TestHelper.TestUserName;
            Password = TestHelper.TestPassword;
        }

        [Fact]
     //  [Ignore("Not Using this feature yet")] // TODO need to look into why this is failing
        public void TestQueryByResultToken()
        {
            var query = new Query(UserName, Password, Mode);

            var startDate = "2017-08-27";
            var endDate = "2017-08-28";

            var resultToken = "100469679705";


            query.GetSingleTransQuery(resultToken, startDate, endDate, "1");

            var result = query.Process();

            var response = query.response;

            Assert.NotNull(response);
            Assert.NotEqual("", response);

        }
    }
}
