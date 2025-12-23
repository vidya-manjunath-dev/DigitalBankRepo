using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAdminService
    {
        ICollection<Customer> GetPendingApprovals();
        (bool Success, string Message) ApproveCustomer(int id);
        (bool Success, string Message) RejectCustomer(int id);
        ICollection<Account> GetAllAccounts();
        (bool Success, string Message) DeleteAccount(int id);
        ICollection<Transaction> GetHighValueTransactions(decimal threshold);
        ICollection<ServiceRequest> GetAllServiceRequests();
        (bool Success, string Message) UpdateServiceRequestStatus(int id, string status);
        (bool Success, string Message) DeleteServiceRequest(int id);
    }
}
