using System.Net;
using System.Net.Http;
using System.Text;
using ChargingGatewayApi.Tests.Fixtures;
using Newtonsoft.Json;
using Xunit;

namespace ChargingGatewayApi.Tests.Controllers
{
    [Trait("Category", "Integration")]
    [Collection("TestServerCollection")]
    public class ValuesControllerTest
    {
        public ValuesControllerTest(TestServerFixture fixture)
        {
            _client = fixture.Client;
        }

        private readonly HttpClient _client;

        [Fact(DisplayName = "Delete should return a NoContentResult status")]
        public void DeleteShouldReturnNoContentResultStatusCode()
        {
            var response = _client.DeleteAsync("v1/values/1").Result;
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "GetById should return the Id with OK status")]
        public void GetByIdShouldReturnOkStatusCode()
        {
            var response = _client.GetAsync("v1/values/1").Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Get should return all with OK status")]
        public void GetShouldReturnAllWithOkStatusCode()
        {
            var response = _client.GetAsync("v1/values").Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Post should return created with Created status")]
        public void PostShouldReturnWithCreatedStatusCode()
        {
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(new { }), Encoding.UTF8,
                "application/json");
            var response = _client.PostAsync("v1/values", requestContent).Result;
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact(DisplayName = "Put should return a NoContentResult status")]
        public void PutShouldReturnNoContentResultStatusCode()
        {
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(new { }), Encoding.UTF8,
                "application/json");
            var response = _client.PutAsync("v1/values/1", requestContent).Result;
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}