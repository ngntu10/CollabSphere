using System;

namespace CollabSphere.Modules.Notification.Models
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
    }

    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
    }

    public class UpdateNotificationDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
    }
}
