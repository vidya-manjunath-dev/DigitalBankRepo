using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IServiceRequestService
    {
        (bool Success, string Message) CreateServiceRequest(CreateServiceRequestDto dto, int userId);
        ICollection<ServiceRequest> GetMyRequests(int userId);
    }
}
