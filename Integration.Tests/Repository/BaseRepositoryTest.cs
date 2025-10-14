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

        var user = new ApplicationUser
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000041"),
            Username = "username",
            Password = "password"
        };

        var booking = new Booking
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
            Name = "name",
            BookingDate = DateTime.UtcNow,
            ContactNumber = 123,
            Email = "email",
            Approved = false,
            Flexibility = flexibility,
            VehicleSize = vehicleSize
        };

        Context.Bookings.Add(booking);
        Context.RdFlexibilities.Add(flexibility);
        Context.RdVehicleSizes.Add(vehicleSize);
        Context.ApplicationUsers.Add(user);

        await Context.SaveChangesAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.RdFlexibilities.RemoveRange(Context.RdFlexibilities);
        Context.RdVehicleSizes.RemoveRange(Context.RdVehicleSizes);
        Context.ApplicationUsers.RemoveRange(Context.ApplicationUsers);
        await Context.SaveChangesAsync();
    }
}
