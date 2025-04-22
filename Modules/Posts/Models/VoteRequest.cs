using System;
using System.Text.Json.Serialization;

using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Posts.Models;

public class VoteRequest
{
    public Guid UserId { get; set; }
    [JsonPropertyName("voteType")]
    public VoteType VoteType { get; set; }
}
