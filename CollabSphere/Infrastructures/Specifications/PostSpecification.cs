using CollabSphere.Entities.Domain;
using CollabSphere.Infrastructures.Specifications.Impl;

namespace CollabSphere.Modules.Posts.Specifications;

public static class PostSpecification
{
    public static BaseSpecification<Entities.Domain.Post> GetPaginatedPostsByUserId(Guid userId, int skip, int take)
    {
        var spec = new BaseSpecification<Entities.Domain.Post>(x => x.CreatedBy == userId);
        spec.ApplyPaging(skip, take);
        spec.ApplyOrderByDescending(x => x.CreatedOn);

        return spec;
    }

    public static BaseSpecification<Entities.Domain.Post> GetPostCountByUserId(Guid userId)
    {
        return new BaseSpecification<Entities.Domain.Post>(x => x.CreatedBy == userId);
    }
}
