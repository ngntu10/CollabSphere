using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Infrastructures.Repositories;
using CollabSphere.Modules.Notification.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Notification.Service
{
    public interface INotificationService
    {
        Task<NotificationDto> GetNotificationByIdAsync(Guid Id);
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<Entities.Domain.Notification> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<NotificationResponseModel> UpdateNotificationAsync(Guid id, UpdateNotificationDto updateDto, Guid updatedById);
        Task<bool> DeleteNotificationAsync(Guid id, Guid deletedById);
    }

    public class NotificationService : INotificationService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public NotificationService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<NotificationDto> GetNotificationByIdAsync(Guid Id)
        {
            var notification = await _context.Notifications.FindAsync(Id);

            if (notification == null)
                throw new NotFoundException("Notification not found");

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifications = await _context.Notifications.ToListAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<Entities.Domain.Notification> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            if (createNotificationDto == null)
                throw new ArgumentNullException(nameof(createNotificationDto), "CreateNotificationDto không được null.");

            var notification = _mapper.Map<Entities.Domain.Notification>(createNotificationDto)
                ?? throw new InvalidOperationException("Mapping failed.");

            notification.Id = Guid.NewGuid();
            notification.CreatedOn = DateTime.UtcNow;
            notification.UpdatedOn = DateTime.UtcNow;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<NotificationResponseModel> UpdateNotificationAsync(Guid id, UpdateNotificationDto updateDto, Guid updatedById)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
                throw new NotFoundException("Thông báo không tồn tại");

            var user = await _context.Users.FindAsync(updateDto.UserId);

            if (user == null)
                throw new NotFoundException("Người dùng không tồn tại");

            notification.UserId = updateDto.UserId;
            notification.Content = updateDto.Content;
            notification.Link = updateDto.Link;
            notification.IsRead = updateDto.IsRead;
            notification.NotificationType = updateDto.NotificationType;
            notification.UpdatedOn = DateTime.UtcNow;

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return new NotificationResponseModel
            {
                Id = notification.Id,
                UserId = notification.UserId.ToString(),
                Link = notification.Link,
                Content = notification.Content,
                IsRead = notification.IsRead,
                NotificationType = notification.NotificationType,
            };
        }

        public async Task<bool> DeleteNotificationAsync(Guid id, Guid deletedById)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
                throw new NotFoundException("Thông báo không tồn tại");

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
