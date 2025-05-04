using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Auth.Models;

using UserEntity = CollabSphere.Entities.Domain.User;

namespace CollabSphere.Modules.Auth.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserModel, UserEntity>();
    }
}
