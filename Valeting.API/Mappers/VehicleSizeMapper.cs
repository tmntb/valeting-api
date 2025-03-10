﻿using AutoMapper;
using Valeting.Repository.Entities;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.API.Mappers;

public class VehicleSizeMapper : Profile
{
    public VehicleSizeMapper()
    {
        // Api -> Dto
        CreateMap<VehicleSizeApiParameters, PaginatedVehicleSizeDtoRequest>()
            .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => src));
        CreateMap<VehicleSizeApiParameters, VehicleSizeFilterDto>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        // Entity -> Dto
        CreateMap<RdVehicleSize, VehicleSizeDto>();

        // Dto -> Api
        CreateMap<VehicleSizeDto, VehicleSizeApi>();
    }
}
