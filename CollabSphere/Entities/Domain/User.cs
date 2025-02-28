
using CollabSphere.Common;
using CollabSphere.Entities.Domain;

namespace CollabSphere.Entities;

public class User: BaseEntity , IAuditedEntity
{   
    public string Username { get ; set ;} = null!;
    public string Email {get; set;} = null!;
    public DateTime CreateAt {get; set;}
    public DateTime? UpdatedAt {get; set;}
    public int Reputation { get; set;}
    public string Role {get; set;} =null!;
    public string Avatar { get; set; } = null!;
    public virtual ICollection <Post> Posts {get; set;} = new List<Post>();
}

