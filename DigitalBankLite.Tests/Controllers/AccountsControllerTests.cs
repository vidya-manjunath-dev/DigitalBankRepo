using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DigitalBankLite.API.Controllers;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DigitalBankLite.Tests.Controllers
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private Mock<IAccountService> _mockAccountService;
        private AccountsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountsController(_mockAccountService.Object);

            // Mock User Context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Test]
        public void GetAccounts_ReturnsOk_WithUserAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, CustomerId = 1, AccountNumber = "123" }
            };
            _mockAccountService.Setup(s => s.GetAccounts(1)).Returns(accounts);

            // Act
            var result = _controller.GetAccounts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedAccounts = okResult?.Value as List<Account>;
            Assert.That(returnedAccounts?.Count, Is.EqualTo(1));
        }

        [Test]
        public void Deposit_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            int accountId = 1;
            decimal amount = 100;
            var transaction = new Transaction { Id = 1, Amount = 100 };
            
            _mockAccountService.Setup(s => s.Deposit(accountId, amount, 1))
                .Returns((true, "Deposit successful", transaction));

            // Act
            var result = _controller.Deposit(accountId, amount);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            
            var value = okResult?.Value;
            var message = value?.GetType().GetProperty("message")?.GetValue(value, null) as string;
            
            Assert.That(message, Is.EqualTo("Deposit successful"));
        }

        [Test]
        public void Deposit_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            int accountId = 1;
            decimal amount = -100;
            
            _mockAccountService.Setup(s => s.Deposit(accountId, amount, 1))
                .Returns((false, "Invalid amount", null));

            // Act
            var result = _controller.Deposit(accountId, amount);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            
            var value = badRequestResult?.Value;
            var message = value?.GetType().GetProperty("message")?.GetValue(value, null) as string;
            
            Assert.That(message, Is.EqualTo("Invalid amount"));
        }
    }
}
