namespace Valeting.Mappers;

public static class MapperRegistration
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(BookingMapper));
        services.AddAutoMapper(typeof(VehicleSizeMapper));
        services.AddAutoMapper(typeof(FlexibilityMapper));
        services.AddAutoMapper(typeof(UserMapper));
    }
}