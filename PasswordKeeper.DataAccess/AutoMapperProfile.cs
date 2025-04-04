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
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>().ForMember(f => f.Id, o => o.Ignore());
        CreateMap<Collection, CollectionDto>().ForMember(d => d.AccessKey, s => s.Ignore()).ReverseMap();
    }
}