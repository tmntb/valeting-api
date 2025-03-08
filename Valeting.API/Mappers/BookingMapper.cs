using AutoMapper;
using Valeting.API.Models.Booking;
using Valeting.Repository.Entities;
using Valeting.Common.Models.Booking;

namespace Valeting.API.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        // Api -> Dto
        CreateMap<CreateBookingApiRequest, CreateBookingDtoRequest>();

        // Dto -> Entity
        CreateMap<BookingDto, Booking>();
    }
}