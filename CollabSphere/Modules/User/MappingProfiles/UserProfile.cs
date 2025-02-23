using AutoMapper;

using CollabSphere.Modules.User.Config;
using CollabSphere.Modules.User.Models;

namespace CollabSphere.Modules.User.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserModel, ApplicationUser>();
    }
}
