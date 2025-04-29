using System;
using System.ComponentModel.DataAnnotations;

namespace CollabSphere.Modules.Comment.Models;

public class UpdateCommentRequest
{
    [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
    public string Content { get; set; }
}
