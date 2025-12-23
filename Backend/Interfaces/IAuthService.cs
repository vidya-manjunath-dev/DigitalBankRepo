using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAuthService
    {
        (bool Success, string Message, Customer? Customer) Register(RegisterDto dto);
        (bool Success, string Message, LoginResponseDto? Response) Login(LoginDto dto);
        void SeedAdmin();
        void ResetAdminPassword();
        ICollection<object> DebugUsers();
        object? GetProfile(int userId);
    }
}
