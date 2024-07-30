using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using CustomerManagement.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomerManagement.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;

namespace CustomerManagement.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<HomeController>> _mockLogger;
        private HomeController _controller;
        private List<Customer> _testCustomers;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<HomeController>>();

            _controller = new HomeController(_mockHttpClientFactory.Object, _mockLogger.Object);

            _testCustomers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1980, 1, 1), Phone = "1234567890", Address = "123 Main St" },
                new Customer { Id = 2, Name = "Jane Doe", Email = "jane.doe@example.com", DateOfBirth = new DateTime(1990, 2, 2), Phone = "0987654321", Address = "456 Elm St" }
            };
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfCustomers()
        {
            // Arrange
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_testCustomers));
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<IEnumerable<Customer>>(viewResult.Model);
            var customers = viewResult.Model as IEnumerable<Customer>;
            Assert.AreEqual(2, customers.Count());
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly List<Customer> _testCustomers;

            public FakeHttpMessageHandler(List<Customer> testCustomers)
            {
                _testCustomers = testCustomers;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(_testCustomers))
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return Task.FromResult(response);
            }
            
        }

        [TearDown]
        public void TearDown()
        {
            //_context.Dispose();
            _controller.Dispose();
        }

        public void Dispose()
        {
            //_context?.Dispose();
            _controller?.Dispose();
        }
    }
}
