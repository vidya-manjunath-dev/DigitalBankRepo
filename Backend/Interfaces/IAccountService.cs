using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAccountService
    {
        ICollection<Account> GetAccounts(int userId);
        ICollection<Transaction> GetTransactions(int accountId, int userId);
        ICollection<object> GetAllTransactions(int userId);
        ICollection<object> GetMiniStatement(int userId);
        byte[] GetDownloadStatement(int userId);
        (bool Success, string Message, Transaction? Transaction) Deposit(int accountId, decimal amount, int userId);
    }
}
