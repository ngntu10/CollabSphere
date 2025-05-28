using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Notification.Models;
using CollabSphere.Modules.Notification.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CollabSphere.Modules.Notification.Controllers;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/notification")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _NotificationService;
    private readonly DatabaseContext _context;
    private readonly ILogger<NotificationController> _logger;
    private readonly IMapper _mapper;

    public NotificationController(INotificationService NotificationService, DatabaseContext context, ILogger<NotificationController> logger, IMapper mapper)
    {
        _NotificationService = NotificationService;
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotificationById(Guid notificationId)
    {
        try
        {
            var notification = await _NotificationService.GetNotificationByIdAsync(notificationId);

            return Ok(ApiResponse<NotificationDto>.Success(
                StatusCodes.Status200OK,
                notification,
                "Lấy nội dung thông báo thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        try
        {
            var notifications = await _NotificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách thông báo");
            return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createNotificationDto)
    {
        if (createNotificationDto == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu tạo Notification không hợp lệ" }
            ));
        }

        var notification = await _NotificationService.CreateNotificationAsync(createNotificationDto);
        if (notification == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Không thể tạo thông báo" }
            ));
        }

        var user = await _context.Users.FindAsync(createNotificationDto.UserId);
        if (user == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Không tìm thấy người dùng" }
            ));
        }

        notification.UserId = createNotificationDto.UserId;
        notification.Content = createNotificationDto.Content;
        notification.Link = createNotificationDto.Link;
        notification.IsRead = createNotificationDto.IsRead;
        notification.NotificationType = createNotificationDto.NotificationType;

        var response = _mapper.Map<NotificationDto>(notification);

        return Ok(ApiResponse<NotificationDto>.Success(
            StatusCodes.Status200OK,
            response,
            "Tạo thông báo thành công"
        ));
    }

    [HttpPut("update/{notificationId}")]
    public async Task<IActionResult> UpdateNotification(Guid notificationId, [FromBody] UpdateNotificationModel model)
    {
        if (model == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu cập nhật không hợp lệ" }
            ));
        }
        var updatedByUserId = User.GetUserId();

        var updateDto = _mapper.Map<UpdateNotificationDto>(model);
        var updatedNotification = await _NotificationService.UpdateNotificationAsync(notificationId, updateDto, updatedByUserId);

        return Ok(ApiResponse<NotificationResponseModel>.Success(
            StatusCodes.Status200OK,
            updatedNotification,
            $"Cập nhật người dùng thành công ngày {updatedNotification.UpdatedOn:dd/MM/yyyy HH:mm:ss}"
        ));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var deletedByUserId = User.GetUserId();

        var isDeleted = await _NotificationService.DeleteNotificationAsync(id, deletedByUserId);

        if (!isDeleted)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { "Bài post không tồn tại hoặc bạn không có quyền xóa" }
            ));
        }

        return Ok(ApiResponse<object>.Success(
            StatusCodes.Status200OK,
            null,
            $"Người dùng {deletedByUserId} đã xóa bài post thành công"
        ));
    }
}
