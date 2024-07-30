using CustomerManagement.Controllers;
using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Tests
{
    [TestFixture]
    public class CustomersControllerTests : IDisposable
    {
        private CustomersController _controller;
        private CustomerContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(databaseName: "CustomerList")
                .Options;

            _context = new CustomerContext(options);
            _context.Customers.AddRange(
                new Customer { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new Customer { Id = 2, Name = "Jane Doe", Email = "jane.doe@example.com" }
            );
            _context.SaveChanges();

            _controller = new CustomersController(_context);
        }

        [Test]
        public async Task GetCustomer_ReturnsCustomer()
        {
            var result = await _controller.GetCustomer(1);

            Assert.IsInstanceOf<ActionResult<Customer>>(result);

            var customer1 = result.Value;
            Assert.IsNotNull(customer1);
            Assert.That(customer1.Id, Is.EqualTo(1));

        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
