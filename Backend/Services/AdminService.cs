using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly BankDbContext _context;

        public AdminService(BankDbContext context)
        {
            _context = context;
        }

        public ICollection<Customer> GetPendingApprovals()
        {
            return _context.Customers
                .Where(c => c.KycStatus == "Pending" && c.Role != "Admin")
                .ToList();
        }

        public (bool Success, string Message) ApproveCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return (false, "Customer not found.");

            customer.KycStatus = "Approved";

            // Activate all their accounts
            var accounts = _context.Accounts.Where(a => a.CustomerId == id && a.Status == "Inactive").ToList();
            foreach (var acc in accounts)
            {
                acc.Status = "Active";
                acc.Balance = 0; 
            }

            _context.SaveChanges();
            return (true, "Customer approved and accounts activated.");
        }

        public (bool Success, string Message) RejectCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return (false, "Customer not found.");

            customer.KycStatus = "Rejected";
            _context.SaveChanges();
            return (true, "Customer KYC rejected.");
        }

        public ICollection<Account> GetAllAccounts()
        {
            return _context.Accounts
                .Include(a => a.Customer)
                .ToList();
        }

        public (bool Success, string Message) DeleteAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) return (false, "Account not found.");

            if (account.Balance > 0)
            {
                return (false, "Cannot delete an account with a non-zero balance.");
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return (true, "Account deleted successfully.");
        }

        public ICollection<Transaction> GetHighValueTransactions(decimal threshold)
        {
            return _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a!.Customer)
                .Where(t => t.Amount > threshold)
                .OrderByDescending(t => t.TxnDateTime)
                .ToList();
        }

        public ICollection<ServiceRequest> GetAllServiceRequests()
        {
            return _context.ServiceRequests
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public (bool Success, string Message) UpdateServiceRequestStatus(int id, string status)
        {
            var request = _context.ServiceRequests.Find(id);
            if (request == null) return (false, "Request not found.");

            request.Status = status;
            request.UpdatedDate = DateTime.UtcNow;

            _context.SaveChanges();
            return (true, "Status updated.");
        }

        public (bool Success, string Message) DeleteServiceRequest(int id)
        {
            var request = _context.ServiceRequests.Find(id);
            if (request == null) return (false, "Request not found.");

            if (request.Status != "Closed")
            {
                return (false, "Only closed requests can be deleted.");
            }

            _context.ServiceRequests.Remove(request);
            _context.SaveChanges();
            return (true, "Service request deleted.");
        }
    }
}
