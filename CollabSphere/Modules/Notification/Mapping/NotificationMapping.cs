using System.Linq;
using System.Security.Claims;

using AutoMapper;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Notification.Models;

using NotificationEntity = CollabSphere.Entities.Domain.Notification;

namespace CollabSphere.Modules.Notification.Mapping
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            // Từ Notification entity sang NotificationDto
            CreateMap<NotificationEntity, NotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.NotificationType, opt => opt.MapFrom((src => src.NotificationType)));

            // Từ CreateNotificationDto sang Notification (cho việc tạo mới)
            CreateMap<CreateNotificationDto, NotificationEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.NotificationType, opt => opt.MapFrom((src => src.NotificationType)));

            CreateMap<UpdateNotificationDto, NotificationEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.NotificationType, opt => opt.MapFrom((src => src.NotificationType)));
        }
    }
}
