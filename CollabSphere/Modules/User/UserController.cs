using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollabSphere.Modules.User.Models;
using CollabSphere.Modules.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CollabSphere.Modules.User.Controllers;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _UserService;
    private readonly DatabaseContext _context;

    public UserController(IUserService UserService, DatabaseContext context)
    {
        _UserService = UserService;
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            var user = await _UserService.GetUserByIdAsync(userId);

            return Ok(ApiResponse<PostDto>.Success(
                StatusCodes.Status200OK,
                post,
                "Lấy thông tin người dùng thành công"
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
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _UserService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách User");
            return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (createUserDto == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu tạo User không hợp lệ" }
            ));
        }

        var user = await _UserService.CreateUserAsync(createUserDto);
        if (user == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Không thể tạo User" }
            ));
        }

        var response = new CreateUserModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Gender = user.Gender,
            AvatarId = user.AvatarId,
        };

        return Ok(ApiResponse<CreateUserModel>.Success(
            StatusCodes.Status200OK,
            response,
            "Tạo User thành công"
        ));
    }

    [HttpPut("update/{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserModel model)
    {
        if (model == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu cập nhật không hợp lệ" }
            ));
        }

        var updatedUser = await _UserService.UpdateUserAsync(userId, model);

        return Ok(ApiResponse<UpdateUserModel>.Success(
            StatusCodes.Status200OK,
            updatedUser,
            $"Cập nhật người dùng {updatedUser.UpdatedByUsername} thành công ngày {updatedUser.UpdatedOn:dd/MM/yyyy HH:mm:ss}"
        ));
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var deletedUser = await _UserService.DeleteUserAsync(userId);

        if (!deletedUser)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { "Người dùng không tồn tại hoặc bạn không có quyền xóa" }
            ));
        }

        return Ok(ApiResponse<object>.Success(
            StatusCodes.Status200OK,
            null,
            $"Đã xóa người dùng {deletedUser} thành công"
        ));
    }
}