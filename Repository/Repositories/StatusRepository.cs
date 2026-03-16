using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Status;

namespace Repository.Repositories;

public class StatusRepository(ValetingContext valetingContext) : IStatusRepository
{
    public async Task<StatusDto> GetByCodeAsync(StatusEnum statusCode)
    {
        var rdStatus = await valetingContext.RdStatus.FirstOrDefaultAsync(s => s.Code == statusCode);
        if(rdStatus == null)
            return null;

        return new()
        {
            Id = rdStatus.Id,
            Name = rdStatus.Name,
            Active = rdStatus.Active
        };
    }
}
