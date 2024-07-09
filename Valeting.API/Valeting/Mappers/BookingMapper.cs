using AutoMapper;

using Valeting.ApiObjects.Booking;
using Valeting.Business.Booking;
using Valeting.Repositories.Entities;
using Valeting.Services.Objects.Booking;

namespace Valeting.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        //API -> Service
        CreateMap<CreateBookingApiRequest, CreateBookingSVRequest>();

        //Service -> DTO
        CreateMap<CreateBookingSVRequest, BookingDTO>();

        //DTO -> Entity
        CreateMap<BookingDTO, Booking>();
    }
}