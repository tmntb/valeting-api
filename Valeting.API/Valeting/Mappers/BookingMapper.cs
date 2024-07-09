using AutoMapper;

using Valeting.ApiObjects.Booking;
using Valeting.Services.Objects.Booking;

namespace Valeting.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        //Service -> API

        //API -> Out
        CreateMap<BookingSV, BookingApi>();
    }
}