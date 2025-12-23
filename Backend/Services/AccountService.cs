using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DigitalBankLite.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankDbContext _context;

        public AccountService(BankDbContext context)
        {
            _context = context;
        }

        public ICollection<Account> GetAccounts(int userId)
        {
            return _context.Accounts
                .Where(a => a.CustomerId == userId)
                .ToList();
        }

        public ICollection<Transaction> GetTransactions(int accountId, int userId)
        {
            // Validate ownership
            var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId && a.CustomerId == userId);
            if (account == null)
            {
                return new List<Transaction>();
            }

            return _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TxnDateTime)
                .ToList();
        }

        public ICollection<object> GetAllTransactions(int userId)
        {
            // Join Transactions with Accounts to verify ownership and get details
             var transactions = _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account!.CustomerId == userId)
                .OrderByDescending(t => t.TxnDateTime)
                .Select(t => (object)new
                {
                    t.Id,
                    t.TxnDateTime,
                    t.Description,
                    t.Type,
                    t.Amount,
                    t.BalanceAfterTxn,
                    AccountType = t.Account!.AccountType,
                    AccountNumber = t.Account.AccountNumber
                })
                .ToList();
            
            return transactions;
        }
        
        public ICollection<object> GetMiniStatement(int userId)
        {
            var transactions = _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account!.CustomerId == userId)
                .OrderByDescending(t => t.TxnDateTime)
                .Take(10) // Only last 10
                .Select(t => (object)new
                {
                    t.Id,
                    t.TxnDateTime,
                    t.Description,
                    t.Type,
                    t.Amount,
                    t.BalanceAfterTxn,
                    AccountType = t.Account!.AccountType,
                    AccountNumber = t.Account.AccountNumber
                })
                .ToList();
            
            return transactions;
        }

        public byte[] GetDownloadStatement(int userId)
        {
            var transactions = _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account!.CustomerId == userId)
                .OrderByDescending(t => t.TxnDateTime)
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Date,Account,Type,Description,Amount,Balance After");

            foreach (var t in transactions)
            {
                csv.AppendLine($"{t.TxnDateTime:yyyy-MM-dd HH:mm:ss},{t.Account!.AccountNumber},{t.Type},{t.Description},{t.Amount},{t.BalanceAfterTxn}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public (bool Success, string Message, Transaction? Transaction) Deposit(int accountId, decimal amount, int userId)
        {
             if (amount <= 0)
            {
                return (false, "Deposit amount must be greater than zero.", null);
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId && a.CustomerId == userId);
            if (account == null)
            {
                return (false, "Account not found or access denied.", null);
            }

            if (account.Status != "Active")
            {
                return (false, "Account is inactive.", null);
            }

            account.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = "Credit",
                Description = "Cash Deposit",
                TxnDateTime = DateTime.UtcNow,
                BalanceAfterTxn = account.Balance
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return (true, "Deposit successful.", transaction);
        }
    }
}
