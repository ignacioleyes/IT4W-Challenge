using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IT4W_Challenge.Tests.IntegrationTests
{
    [TestClass]
    public class DeliveryControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetShortestTime_ReturnsCorrectResults()
        {
            // Arrange
            var source = 0;
            var destinations = "1,2,3";
            var url = $"/api/delivery/shortest-time?source={source}&destinations={destinations}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "\"1\": 5");
            StringAssert.Contains(responseContent, "\"2\": 15");
            StringAssert.Contains(responseContent, "\"3\": 18");
        }

        [TestMethod]
        public async Task GetShortestTime_ReturnsBadRequest_OnNegativeCycle()
        {
            // Arrange
            var source = 0;
            var destinations = "1,2,3";
            var url = $"/api/delivery/shortest-time?source={source}&destinations={destinations}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "El grafo contiene ciclos negativos");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
