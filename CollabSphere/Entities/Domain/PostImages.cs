using System;
using System.Text.Json.Serialization;

using CollabSphere.Common;

namespace CollabSphere.Entities.Domain;

public class PostImages : BaseEntity
{

    public Guid? PostId { get; set; }
    public string? ImageID { get; set; }
    [JsonIgnore]
    public virtual Post? Post { get; set; }

}
