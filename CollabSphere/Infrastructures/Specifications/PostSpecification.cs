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
        spec.AddInclude(x => x.Comments);
        spec.AddInclude("Comments.User");
        spec.AddInclude(x => x.Votes);
        spec.AddInclude(x => x.Shares);
        spec.AddInclude(x => x.Reports);
        spec.AddInclude(x => x.PostImages);
        spec.AddInclude(x => x.User);

        return spec;
    }

    public static BaseSpecification<Entities.Domain.Post> GetPostCountByUserId(Guid userId)
    {
        return new BaseSpecification<Entities.Domain.Post>(x => x.CreatedBy == userId);
    }
}
