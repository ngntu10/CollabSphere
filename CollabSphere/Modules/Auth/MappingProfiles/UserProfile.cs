using AutoMapper;

using CollabSphere.Modules.Auth.Config;
using CollabSphere.Modules.Auth.Models;

namespace CollabSphere.Modules.Auth.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserModel, ApplicationUser>();
    }
}
