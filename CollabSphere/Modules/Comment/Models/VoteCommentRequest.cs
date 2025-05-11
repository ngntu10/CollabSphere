using System;
using System.Text.Json.Serialization;

using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Comment.Models
{
    public class VoteCommentRequest
    {
        public Guid UserId { get; set; }

        [JsonPropertyName("voteType")]
        public VoteType VoteType { get; set; }
    }
}
