using AutoMapper;

using Valeting.Models.Booking;
using Valeting.Repository.Entities;
using Valeting.Core.Models.Booking;
using Valeting.Core.Models.Flexibility;
using Valeting.Core.Models.VehicleSize;
using Valeting.Repository.Models.Booking;
using Valeting.Repository.Models.VehicleSize;
using Valeting.Repository.Models.Flexibility;
using Valeting.Models.Flexibility;
using Valeting.Models.VehicleSize;

namespace Valeting.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        //API -> Service
        CreateMap<CreateBookingApiRequest, CreateBookingSVRequest>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => new FlexibilitySV { Id = src.Flexibility.Id }))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => new VehicleSizeSV { Id = src.VehicleSize.Id }));
        CreateMap<UpdateBookingApiRequest, UpdateBookingSVRequest>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => new FlexibilitySV { Id = src.Flexibility.Id }))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => new VehicleSizeSV { Id = src.VehicleSize.Id }));

        //Service -> DTO
        CreateMap<CreateBookingSVRequest, BookingDTO>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => new FlexibilityDTO { Id = src.Flexibility.Id }))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => new VehicleSizeDTO { Id = src.VehicleSize.Id }));

        CreateMap<UpdateBookingSVRequest, BookingDTO>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => new FlexibilityDTO { Id = src.Flexibility.Id }))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => new VehicleSizeDTO { Id = src.VehicleSize.Id }));

        //DTO -> Entity
        CreateMap<FlexibilityDTO, RdFlexibility>();
        CreateMap<VehicleSizeDTO, RdVehicleSize>();
        CreateMap<BookingDTO, Booking>()
            .ForMember(dest => dest.Flexibility, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleSize, opt => opt.Ignore());

        //Entity -> DTO
        CreateMap<RdFlexibility, FlexibilityDTO>();
        CreateMap<RdVehicleSize, VehicleSizeDTO>();
        CreateMap<Booking, BookingDTO>();

        //DTO -> Service
        CreateMap<FlexibilityDTO, FlexibilitySV>();
        CreateMap<VehicleSizeDTO, VehicleSizeSV>();
        CreateMap<BookingDTO, BookingSV>();

        //Service -> API
        CreateMap<CreateBookingSVResponse, CreateBookingApiResponse>();
        CreateMap<FlexibilitySV, FlexibilityApi>();
        CreateMap<VehicleSizeSV, VehicleSizeApi>();
        CreateMap<BookingSV, BookingApi>();
    }
}