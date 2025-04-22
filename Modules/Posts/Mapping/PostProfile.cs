using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Posts.Mapping
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            // Chỉ ánh xạ SubredditId khi có giá trị hợp lệ (không phải Guid.Empty hoặc null)
            CreateMap<CreatePostDto, CollabSphere.Entities.Domain.Post>()
                .ForMember(dest => dest.PostImages, opt => opt.Ignore())  // Ignore PostImages during mapping
                .ForMember(dest => dest.Category, opt =>
                    opt.MapFrom(src => string.IsNullOrEmpty(src.Category) ? "General" : src.Category))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));


            CreateMap<CollabSphere.Entities.Domain.Post, PostDto>();
        }
    }
}
