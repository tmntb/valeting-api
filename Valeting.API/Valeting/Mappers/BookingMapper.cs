using AutoMapper;

using Valeting.ApiObjects.Booking;
using Valeting.Business.Booking;
using Valeting.Repositories.Entities;
using Valeting.Services.Objects.Booking;
using Valeting.Services.Objects.Flexibility;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        //API -> Service
        CreateMap<CreateBookingApiRequest, CreateBookingSVRequest>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => new FlexibilitySV { Id = src.Flexibility.Id }))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => new VehicleSizeSV { Id = src.VehicleSize.Id }));

        //Service -> DTO
        CreateMap<CreateBookingSVRequest, BookingDTO>();

        //DTO -> Entity
        CreateMap<BookingDTO, Booking>();

        //Service -> API
        CreateMap<CreateBookingSVResponse, CreateBookingApiResponse>();
    }
}