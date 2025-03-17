using AutoMapper;
using PasswordKeeper.DAO;
using PasswordKeeper.DTO;

namespace PasswordKeeper.DataAccess;

/// <summary>
/// The AutoMapper profile for DAO to DTO.
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// The AutoMapper profile for DAO to DTO.
    /// </summary>
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}