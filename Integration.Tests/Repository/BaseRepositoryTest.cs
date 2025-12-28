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
            Description = "1 day",
            Active = true
        };

        var vehicleSize = new RdVehicleSize
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000031"),
            Description = "Small",
            Active = true
        };

        var role = new RdRole
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000051"),
            Name = RoleEnum.User
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

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Name = "name",
            BookingDate = DateTime.UtcNow,
            Approved = false,
            Flexibility = flexibility,
            VehicleSize = vehicleSize
        };

        Context.Bookings.Add(booking);
        Context.RdFlexibilities.Add(flexibility);
        Context.RdVehicleSizes.Add(vehicleSize);
        Context.ApplicationUsers.Add(user);
        Context.RdRoles.Add(role);

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
