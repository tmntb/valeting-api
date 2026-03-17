using Common.Enums;
using Service.Models.Booking;
using Service.Models.Flexibility;
using Service.Models.Status;
using Service.Models.User;
using Service.Models.VehicleSize;

namespace Service.Tests;

public static class DataFactory
{
    private static readonly Guid BOOKING_ID = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid FLEXIBILITY_ID = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid VEHICLE_SIZE_ID = Guid.Parse("00000000-0000-0000-0000-000000000003"); 
    private static readonly Guid STATUS_ID = Guid.Parse("00000000-0000-0000-0000-000000000004");
    private static readonly Guid USER_ID = Guid.Parse("00000000-0000-0000-0000-000000000005");
    
    public static BookingDto CreateBookingDto(Guid bookingId = default, StatusEnum statusEnum = StatusEnum.PENDING_APPROVAL, DateTime scheduledAt = default)
    {
        return new()
        {
            Id = bookingId == default ? BOOKING_ID : bookingId,
            Reference = "name",
            Status = CreateStatusDto(statusEnum: statusEnum),
            ScheduledAt = scheduledAt == default ? DateTime.Now.AddDays(1) : scheduledAt,
            Flexibility = CreateCreateFlexibilityDto(),
            VehicleSize = CreateVehicleSizeDto(),
            RequiresApproval = true
        };
    }

    public static FlexibilityDto CreateCreateFlexibilityDto(Guid flexibilityId = default)
    {
        return new ()
        {
            Id = flexibilityId == default ? FLEXIBILITY_ID : flexibilityId,
            Name = "name",
            NumberOfMinutes = 60,
            Active = true
        };
    }

    public static VehicleSizeDto CreateVehicleSizeDto(Guid vehicleSizeId = default)
    {
        return new ()
        {
            Id = vehicleSizeId == default ? VEHICLE_SIZE_ID : vehicleSizeId,
            Name = "name",
            Active = true
        };
    }

    public static StatusDto CreateStatusDto(Guid statusId = default, StatusEnum statusEnum = StatusEnum.PENDING_APPROVAL)
    {
        return new ()
        {
            Id = statusId == default ? STATUS_ID : statusId,
            Code = statusEnum,
            Name = "name",
            Active = true
        };
    }

    public static UserDto CreateUserDto(Guid userId = default, RoleEnum roleEnum = RoleEnum.USER)
    {
        return new ()
        {
            Id = userId == default ? USER_ID : userId,
            Role = new()
            {
                Code = roleEnum,
                Name = "name"
            }
        };
    }
}
