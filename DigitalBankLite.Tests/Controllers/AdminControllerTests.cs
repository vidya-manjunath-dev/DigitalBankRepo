using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DigitalBankLite.API.Controllers;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using System.Collections.Generic;

namespace DigitalBankLite.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminService> _mockAdminService;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
        }

        [Test]
        public void GetAllAccounts_ReturnsOk_WithListOfAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountNumber = "123", Balance = 1000 },
                new Account { Id = 2, AccountNumber = "456", Balance = 2000 }
            };
            _mockAdminService.Setup(s => s.GetAllAccounts()).Returns(accounts);

            // Act
            var result = _controller.GetAllAccounts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedAccounts = okResult?.Value as List<Account>;
            Assert.That(returnedAccounts?.Count, Is.EqualTo(2));
        }

        [Test]
        public void ApproveCustomer_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            int customerId = 1;
            _mockAdminService.Setup(s => s.ApproveCustomer(customerId)).Returns((true, "Approved"));

            // Act
            var result = _controller.ApproveCustomer(customerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            // Using reflection to access anonymous type properties
            var value = okResult?.Value;
            var message = value?.GetType().GetProperty("message")?.GetValue(value, null) as string;
            
            Assert.That(message, Is.EqualTo("Approved"));
        }

        [Test]
        public void DeleteAccount_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            int accountId = 1;
            _mockAdminService.Setup(s => s.DeleteAccount(accountId)).Returns((true, "Deleted"));

            // Act
            var result = _controller.DeleteAccount(accountId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            var value = okResult?.Value;
            var message = value?.GetType().GetProperty("message")?.GetValue(value, null) as string;
            
            Assert.That(message, Is.EqualTo("Deleted"));
        }

        [Test]
        public void DeleteAccount_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            int accountId = 1;
            _mockAdminService.Setup(s => s.DeleteAccount(accountId)).Returns((false, "Failed"));

            // Act
            var result = _controller.DeleteAccount(accountId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            
            var value = badRequestResult?.Value;
            var message = value?.GetType().GetProperty("message")?.GetValue(value, null) as string;
            
            Assert.That(message, Is.EqualTo("Failed"));
        }
    }
}
