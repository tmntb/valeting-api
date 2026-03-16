using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Integration.Tests.Repository;

public class BaseRepositoryTest : IAsyncLifetime
{
    protected readonly ValetingContext Context;

    protected BaseRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ValetingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new ValetingContext(options);
    }

    public async Task InitializeAsync()
    {
        Context.Database.EnsureCreated();

        await SeedDefaultDataAsync();
    }

    public async Task DisposeAsync()
    {
        await ClearDatabaseAsync();
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }

    protected async Task SeedDefaultDataAsync()
    {
        var flexibility = new RdFlexibility
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
            Name = "1 day",
            Active = true
        };

        var vehicleSize = new RdVehicleSize
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000031"),
            Name = "Small",
            Active = true
        };

        var role = new RdRole
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000051"),
            Name = "User",
            Code = RoleEnum.USER
        };

        var user = new ApplicationUser
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000041"),
            Username = "username",
            PasswordHash = "password",
            ContactNumber = 1234567890,
            Email = "test@example.com",
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.MinValue,
            LastLoginAt = DateTime.MinValue
        };

        var status = new RdStatus
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000061"),
            Code = StatusEnum.PENDING_APPROVAL,
            Name = "Pending"
        };

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Reference = "name",
            Customer = user,
            Flexibility = flexibility,
            VehicleSize = vehicleSize,
            ScheduledAt = DateTime.UtcNow,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.MinValue,
            DecisionAt = DateTime.MinValue,
            DecisionBy = null,
            RequiresApproval = false,
            Notes = "notes"
        };

        Context.Bookings.Add(booking);
        Context.RdFlexibilities.Add(flexibility);
        Context.RdVehicleSizes.Add(vehicleSize);
        Context.ApplicationUsers.Add(user);
        Context.RdRoles.Add(role);
        Context.RdStatus.Add(status);

        await Context.SaveChangesAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.RdFlexibilities.RemoveRange(Context.RdFlexibilities);
        Context.RdVehicleSizes.RemoveRange(Context.RdVehicleSizes);
        Context.ApplicationUsers.RemoveRange(Context.ApplicationUsers);
        Context.RdRoles.RemoveRange(Context.RdRoles);
        await Context.SaveChangesAsync();
    }
}
