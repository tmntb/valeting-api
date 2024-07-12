﻿using AutoMapper;

using Valeting.Models.VehicleSize;
using Valeting.Repository.Entities;
using Valeting.Core.Models.VehicleSize;
using Valeting.Repository.Models.VehicleSize;

namespace Valeting.Mappers;

public class VehicleSizeMapper : Profile
{
    public VehicleSizeMapper()
    {
        //API -> Service
        CreateMap<VehicleSizeApiParameters, PaginatedVehicleSizeSVRequest>()
            .ForMember(dest => dest.Filter, act => act.MapFrom(src => src));

        CreateMap<VehicleSizeApiParameters, VehicleSizeFilterSV>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

        //Service -> DTO
        CreateMap<VehicleSizeFilterSV, VehicleSizeFilterDTO>();

        //Entity -> DTO
        CreateMap<RdVehicleSize, VehicleSizeDTO>();

        //DTO -> Service
        CreateMap<VehicleSizeDTO, VehicleSizeSV>();
        CreateMap<VehicleSizeListDTO, PaginatedVehicleSizeSVResponse>();

        //Service -> API
        CreateMap<VehicleSizeSV, VehicleSizeApi>();
    }
}
