using System;

namespace CollabSphere.Modules.Notification.Models;

public class NotificationResponseModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
    public string Link { get; set; }
    public bool IsRead { get; set; }
    public string NotificationType { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}

public class CreateNotificationModel
{
    public string UserId { get; set; }
    public string Content { get; set; }
    public string Link { get; set; }
    public bool IsRead { get; set; }
    public string NotificationType { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}

public class UpdateNotificationModel
{
    public string UserId { get; set; }
    public string Content { get; set; }
    public string Link { get; set; }
    public bool IsRead { get; set; }
    public string NotificationType { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}
