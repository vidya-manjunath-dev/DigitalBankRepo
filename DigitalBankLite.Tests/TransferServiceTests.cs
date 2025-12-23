using DigitalBankLite.API.Services;
using DigitalBankLite.API.Models;
using DigitalBankLite.API.DTOs;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Security.Claims;

namespace DigitalBankLite.Tests
{
    [TestFixture]
    public class TransferServiceTests
    {
        private BankDbContext _context;
        private TransferService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;
            
            _context = new BankDbContext(options);
            _service = new TransferService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Transfer_ShouldSucceed_WhenValid()
        {
            // Arrange
            var user = new Customer { Id = 1, Name = "Test User", Email = "test@example.com", PasswordHash = "hash" };
            var fromAccount = new Account { Id = 1, CustomerId = 1, Balance = 2000, AccountNumber = "DB60422219", AccountType = "Savings", Status = "Active" };
            var toAccount = new Account { Id = 2, CustomerId = 1, Balance = 0, AccountNumber = "DB11444815", AccountType = "Current", Status = "Active" };

            _context.Customers.Add(user);
            _context.Accounts.Add(fromAccount);
            _context.Accounts.Add(toAccount);
            _context.SaveChanges();

            var dto = new TransferDto { FromAccountId = 1, ToAccountId = 2, Amount = 1000, Remarks = "Test" };

            // Act
            var result = _service.Transfer(dto, 1);

            // Assert
            Assert.That(result.Success, Is.True, result.Message);
            Assert.That(result.NewBalance, Is.EqualTo(1000));
            
            var updatedFrom = _context.Accounts.Find(1);
            var updatedTo = _context.Accounts.Find(2);
            
            Assert.That(updatedFrom, Is.Not.Null);
            Assert.That(updatedTo, Is.Not.Null);
            
            Assert.That(updatedFrom!.Balance, Is.EqualTo(1000));
            Assert.That(updatedTo!.Balance, Is.EqualTo(1000));
        }
    }
}
