using AutoMapper;

using Valeting.Models.Booking;
using Valeting.Repository.Entities;
using Valeting.Core.Models.Booking;
using Valeting.Core.Models.Flexibility;
using Valeting.Core.Models.VehicleSize;
using Valeting.Repository.Models.Booking;
using Valeting.Repository.Models.VehicleSize;
using Valeting.Repository.Models.Flexibility;

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
        CreateMap<FlexibilityDTO, RdFlexibility>();
        CreateMap<VehicleSizeDTO, RdVehicleSize>();
        CreateMap<BookingDTO, Booking>();

        //Entity -> DTO
        CreateMap<RdFlexibility, FlexibilityDTO>();
        CreateMap<RdVehicleSize, VehicleSizeDTO>();
        CreateMap<Booking, BookingDTO>();

        //DTO -> Service

        //Service -> API
        CreateMap<CreateBookingSVResponse, CreateBookingApiResponse>();
    }
}