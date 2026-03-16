using Common.Enums;
using Service.Models.Status;

namespace Service.Interfaces;

public interface IStatusRepository
{
    Task<StatusDto> GetByCodeAsync(StatusEnum statusCode);
}
