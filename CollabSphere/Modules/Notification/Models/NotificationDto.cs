using System;

namespace CollabSphere.Modules.Notification.Models
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string IsRead { get; set; }
        public string NotificationType { get; set; }
    }

    public class CreateNotificationDto
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string IsRead { get; set; }
        public string NotificationType { get; set; }
    }

    public class UpdateNotificationDto
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string IsRead { get; set; }
        public string NotificationType { get; set; }
    }
}
