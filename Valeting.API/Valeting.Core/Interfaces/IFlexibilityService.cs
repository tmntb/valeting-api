﻿using Valeting.Common.Models.Flexibility;

namespace Valeting.Core.Interfaces;

public interface IFlexibilityService
{
    Task<PaginatedFlexibilityDtoResponse> GetAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest);
    Task<GetFlexibilityDtoResponse> GetByIdAsync(GetFlexibilityDtoRequest getFlexibilityDtoRequest);
}