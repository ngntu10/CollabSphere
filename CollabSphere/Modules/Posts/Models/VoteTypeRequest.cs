using System.Text.Json.Serialization;

using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Posts.Models;

public class VoteTypeRequest
{
    [JsonPropertyName("voteType")]
    public VoteType VoteType { get; set; }
}
