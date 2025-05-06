namespace CollabSphere.Modules.Posts.Models;

public class PostImageDto
{
    public Guid? Id { get; set; } // null nếu là ảnh mới thêm
    public string ImageID { get; set; }
    public bool IsDeleted { get; set; } = false; // để biết ảnh nào cần xoá
}
