using AutoMapper;
using Valeting.API.Models.User;
using Valeting.Common.Models.User;
using Valeting.Repository.Entities;

namespace Valeting.API.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        #region Api -> Dto
        CreateMap<LoginApiRequest, ValidateLoginDtoRequest>();
        CreateMap<LoginApiRequest, GenerateTokenJWTDtoRequest>();
        CreateMap<RegisterApiRequest, RegisterDtoRequest>();
        #endregion

        #region Entity -> Dto
        CreateMap<ApplicationUser, UserDto>();
        #endregion

        #region Dto -> Entity
        CreateMap<UserDto, ApplicationUser>();
        #endregion

        #region Dto -> Api
        CreateMap<GenerateTokenJWTDtoResponse, LoginApiResponse>();
        #endregion
    }
}