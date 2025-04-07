using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Posts.Models
{

    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public Guid UserId { get; set; }

        public List<string>? PostImages { get; set; }
    }
}
