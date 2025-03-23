using AutoMapper;
using Valeting.API.Models.Booking;
using Valeting.API.Models.Flexibility;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Models.Booking;
using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.VehicleSize;
using Valeting.Repository.Entities;

namespace Valeting.API.Mappers;

public class BookingMapper : Profile
{
    public BookingMapper()
    {
        #region Api -> Dto
        CreateMap<FlexibilityApi, FlexibilityDto>();
        CreateMap<VehicleSizeApi, VehicleSizeDto>();
        CreateMap<CreateBookingApiRequest, CreateBookingDtoRequest>();
        CreateMap<UpdateBookingApiRequest, UpdateBookingDtoRequest>();
        CreateMap<BookingApiParameters, PaginatedBookingDtoRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));
        CreateMap<BookingApiParameters, BookingFilterDto>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
        #endregion

        #region Dto -> Dto
        CreateMap<CreateBookingDtoRequest, BookingDto>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => src.Flexibility != null ? new FlexibilityDto { Id = src.Flexibility.Id } : null))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => src.VehicleSize != null ? new VehicleSizeDto { Id = src.VehicleSize.Id } : null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Approved, opt => opt.MapFrom(_ => false));
        CreateMap<UpdateBookingDtoRequest, BookingDto>()
            .ForMember(dest => dest.Flexibility, opt => opt.MapFrom(src => src.Flexibility != null ? new FlexibilityDto { Id = src.Flexibility.Id } : null))
            .ForMember(dest => dest.VehicleSize, opt => opt.MapFrom(src => src.VehicleSize != null ? new VehicleSizeDto { Id = src.VehicleSize.Id } : null));
        #endregion

        #region Dto -> Entity
        CreateMap<FlexibilityDto, RdFlexibility>();
        CreateMap<VehicleSizeDto, RdVehicleSize>();
        CreateMap<BookingDto, Booking>()
            .ForMember(dest => dest.Flexibility, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleSize, opt => opt.Ignore());
        #endregion

        #region Entity -> Dto
        CreateMap<Booking, BookingDto>();
        #endregion

        #region Dto -> Api
        CreateMap<BookingDto, BookingApi>();
        CreateMap<CreateBookingDtoResponse, CreateBookingApiResponse>();
        #endregion
    }
}