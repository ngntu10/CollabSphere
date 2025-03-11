using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Posts.Dtos;

namespace CollabSphere.Modules.Posts.Mapping
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<CreatePostDto, CollabSphere.Entities.Domain.Post>();
            CreateMap<CollabSphere.Entities.Domain.Post, PostDto>();
        }
    }
}
