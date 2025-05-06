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
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom((src => src.UserName)))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            // Từ CreateUserDto sang User (cho việc tạo mới)
            // CreateMap<CreateUserDto, UserEntity>()
            //     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            //     .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //     .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId))
            //     .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<UpdateUserDto, UserEntity>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(src => src.AvatarId))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}
