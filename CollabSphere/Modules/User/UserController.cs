using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Exceptions;
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
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper;

    public UserController(IUserService UserService, DatabaseContext context, ILogger<UserController> logger, IMapper mapper)
    {
        _UserService = UserService;
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            var user = await _UserService.GetUserByIdAsync(userId);

            return Ok(ApiResponse<UserDto>.Success(
                StatusCodes.Status200OK,
                user,
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

    [HttpGet("getUser/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        try
        {
            var user = await _UserService.GetUserByUsernameAsync(username);

            return Ok(ApiResponse<UserDto>.Success(
                StatusCodes.Status200OK,
                user,
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

    // [HttpPost]
    // public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    // {
    //     if (createUserDto == null)
    //     {
    //         return BadRequest(ApiResponse<object>.Failure(
    //             StatusCodes.Status400BadRequest,
    //             new List<string> { "Dữ liệu tạo User không hợp lệ" }
    //         ));
    //     }

    //     var user = await _UserService.CreateUserAsync(createUserDto);
    //     if (user == null)
    //     {
    //         return BadRequest(ApiResponse<object>.Failure(
    //             StatusCodes.Status400BadRequest,
    //             new List<string> { "Không thể tạo User" }
    //         ));
    //     }

    //     user.UserName = createUserDto.UserName;
    //     user.Email = createUserDto.Email;
    //     user.PhoneNumber = createUserDto.PhoneNumber;
    //     user.Gender = createUserDto.Gender;
    //     user.AvatarId = createUserDto.AvatarId;

    //     var response = _mapper.Map<UserDto>(user);

    //     return Ok(ApiResponse<UserDto>.Success(
    //         StatusCodes.Status200OK,
    //         response,
    //         "Tạo User thành công"
    //     ));
    // }

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
        var updatedByUserId = User.GetUserId();

        var updateDto = _mapper.Map<UpdateUserDto>(model);
        var updatedUser = await _UserService.UpdateUserAsync(userId, updateDto, updatedByUserId);

        return Ok(ApiResponse<UserResponseModel>.Success(
            StatusCodes.Status200OK,
            updatedUser,
            $"Cập nhật người dùng thành công ngày {updatedUser.UpdatedOn:dd/MM/yyyy HH:mm:ss}"
        ));
    }
}
