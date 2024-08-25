using AutoMapper;

using Valeting.Models.User;
using Valeting.Core.Models.User;
using Valeting.Repository.Entities;
using Valeting.Repository.Models.User;

namespace Valeting.Mappers;
public class UserMapper : Profile
{
    public UserMapper()
    {
        //API -> Service
        CreateMap<ValidateLoginApiRequest, ValidateLoginSVRequest>();
        CreateMap<ValidateLoginApiRequest, GenerateTokenJWTSVRequest>();

        //Entity -> DTO
        CreateMap<ApplicationUser, UserDTO>();

        //Service -> API
        CreateMap<GenerateTokenJWTSVResponse, ValidateLoginApiResponse>();
    }
}