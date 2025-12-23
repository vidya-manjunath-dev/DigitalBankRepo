using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly BankDbContext _context;

        public ServiceRequestService(BankDbContext context)
        {
            _context = context;
        }

        public (bool Success, string Message) CreateServiceRequest(CreateServiceRequestDto dto, int userId)
        {
            var request = new ServiceRequest
            {
                CustomerId = userId,
                Title = dto.Title,
                Description = dto.Description,
                Status = "Open",
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(request);
            _context.SaveChanges();

            return (true, "Service request created successfully.");
        }

        public ICollection<ServiceRequest> GetMyRequests(int userId)
        {
            return _context.ServiceRequests
                .Where(r => r.CustomerId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }
    }
}
