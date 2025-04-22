using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Auth.Models;

namespace CollabSphere.Modules.Auth.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserModel, User>();
    }
}
