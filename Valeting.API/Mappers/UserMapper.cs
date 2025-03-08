using AutoMapper;
using Valeting.API.Models.User;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;

namespace Valeting.API.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        // Api -> Dto
        CreateMap<ValidateLoginApiRequest, ValidateLoginDtoRequest>();
        CreateMap<ValidateLoginApiRequest, GenerateTokenJWTDtoRequest>();

        // Entity -> Dto
        CreateMap<ApplicationUser, UserDto>();

        // Dto -> Api
        CreateMap<GenerateTokenJWTDtoResponse, ValidateLoginApiResponse>();
    }
}