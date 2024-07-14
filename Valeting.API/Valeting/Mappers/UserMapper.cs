using AutoMapper;

using Valeting.Repository.Entities;
using Valeting.Repository.Models.User;

namespace Valeting.Mappers;
public class UserMapper : Profile
{
    public UserMapper()
    {
        //Entity -> DTO
        CreateMap<ApplicationUser, UserDTO>();
    }
}