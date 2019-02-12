using System.IO;
using BluePayLibrary.Report;
using Xunit;

namespace BluePayLibrary.Tests
{
    [Trait("Category", "Integration")]
    [Collection("TestServerCollection")]
    public class DailyReportTest
    {
        private string UserName = "";
        private string Password = "";

        private const string Mode = "TEST"; // run all tests in Test mode (use "LIVE" for production calls)
        private const string ResponseVersion = "4";

        public DailyReportTest()
        {
            UserName = TestHelper.TestUserName;
            Password = TestHelper.TestPassword;
        }

        [Fact]
      //  [Ignore("Not using this functionality")]
        public void TestDailyReport()
        {
            var report = new DailyReport(UserName, Password, Mode);

            var startDate = "2017-09-01";
            var endDate = "2017-09-02";

           report.GetTransactionReport(startDate, endDate, "1");

            var result = report.Process();

            var response = report.response;

            // save data to file
            var rptFile = @"d:\output\TestDailyReport.csv";
            using (TextWriter writer = File.CreateText(rptFile))
            {
                 writer.Write(response);
            }

        }
    }
}
