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
        CreateMap<LoginApiRequest, ValidateLoginDtoRequest>();
        CreateMap<LoginApiRequest, GenerateTokenJWTDtoRequest>();
        CreateMap<RegisterApiRequest, RegisterDtoRequest>();

        // Entity -> Dto
        CreateMap<ApplicationUser, UserDto>();

        // Dto -> Entity
        CreateMap<UserDto, ApplicationUser>();

        // Dto -> Api
        CreateMap<GenerateTokenJWTDtoResponse, LoginApiResponse>();
    }
}