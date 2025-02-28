
using CollabSphere.Common;

namespace CollabSphere.Entities.Domain;

public class Post: BaseEntity, IAuditedEntity
{
    public string Title {get; set;} = null!;
    public string Content {get;set;} = null!;
    public string ThumbnaiUrl {get;set;} = null!;
    public DateTime CreateAt {get; set;}
    public DateTime? UpdatedAt {get; set;}
    public Guid UserId {get; set;}
    public virtual User User {get; set;} = null!;

}
