using System.Linq;
using System.Security.Claims;

using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.User.Models;

using UserEntity = CollabSphere.Entities.Domain.User;

namespace CollabSphere.Modules.User.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // Từ User entity sang UserDto
            CreateMap<UserEntity, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.Items.ContainsKey("Claims") && context.Items["Claims"] is Claim[] claims)
                    {
                        return claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    }
                    return null;
                }))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.Items.ContainsKey("Claims") && context.Items["Claims"] is Claim[] claims)
                    {
                        return claims.FirstOrDefault(c => c.Type == "gender")?.Value;
                    }
                    return null;
                }));

            // Từ CreateUserDto sang User (cho việc tạo mới)
            CreateMap<CreateUserDto, UserEntity>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId));

            CreateMap<UpdateUserDto, UserEntity>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId));
        }
    }
}
