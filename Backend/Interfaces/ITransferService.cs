using DigitalBankLite.API.DTOs;

namespace DigitalBankLite.API.Interfaces
{
    public interface ITransferService
    {
        (bool Success, string Message, decimal? NewBalance) Transfer(TransferDto dto, int userId);
    }
}
