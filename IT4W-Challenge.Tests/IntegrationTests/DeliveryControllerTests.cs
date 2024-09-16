using IT4W_Challenge.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(new DeliveryController(includeNegativeCycle: false));
                });
            });
            var client = factory.CreateClient();

            var source = 0;
            var url = $"/api/delivery/shortest-path?source={source}&destinations=1&destinations=2&destinations=3";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "\"1\":5");
            StringAssert.Contains(responseContent, "\"2\":7");
            StringAssert.Contains(responseContent, "\"3\":9");
        }

        [TestMethod]
        public async Task GetShortestTime_ReturnsBadRequest_OnNegativeCycle()
        {
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(new DeliveryController(includeNegativeCycle: true));
                });
            });
            var client = factory.CreateClient();
            // Arrange
            var source = 0;
            var url = $"/api/delivery/shortest-time?source={source}&destinations=1&destinations=2&destinations=3";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "El grafo contiene ciclos negativos en los tiempos de entrega.");
        }

        [TestMethod]
        public async Task GetShortestPath_ReturnsCorrectResults()
        {
            // Arrange
            var source = 0;
            var url = $"/api/delivery/shortest-path?source={source}&destinations=1&destinations=2&destinations=3";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "\"1\":5");
            StringAssert.Contains(responseContent, "\"2\":7");
            StringAssert.Contains(responseContent, "\"3\":9");
        }

        [TestMethod]
        public async Task GetShortestPath_ReturnsBadRequest_OnMissingDestination()
        {
            // Arrange
            var source = 0;
            var url = $"/api/delivery/shortest-path?source={source}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(responseContent, "The 'destinations' parameter is required and cannot be empty.");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
