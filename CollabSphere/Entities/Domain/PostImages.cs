using System;

using CollabSphere.Common;

namespace CollabSphere.Entities.Domain;

public class PostImages : BaseEntity
{

    public Guid? PostId { get; set; }
    public string? ImageID { get; set; }
    public virtual Post? Post { get; set; }

}
