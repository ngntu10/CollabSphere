using System;
using System.ComponentModel.DataAnnotations;

public class CreateMessageRequest
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Subject { get; set; }

    [Required]
    [MaxLength(ChatConstants.MAX_MESSAGE_LENGTH)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MessageType { get; set; } = ChatConstants.MESSAGE_TYPE_TEXT;
}
