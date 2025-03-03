using CollabSphere.Common;
using CollabSphere.Modules.User.Models;
using CollabSphere.Modules.User.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.Account;

public class UsersController : ApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginUserModel loginUserModel)
    {
        var result = await _userService.LoginAsync(loginUserModel);

        return Ok(ApiResponse<LoginResponseModel>.Success(
            StatusCodes.Status200OK,
            result,
            "Đăng nhập thành công"
        ));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(ApiResponse<BaseResponseModel>.Success(
            StatusCodes.Status200OK,
            new BaseResponseModel { }));
    }

    [HttpPut("{id:guid}/changePassword")]
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordModel changePasswordModel)
    {
        return Ok(ApiResponse<BaseResponseModel>.Success(StatusCodes.Status200OK,
            await _userService.ChangePasswordAsync(id, changePasswordModel)));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserModel model)
    {
        var result = await _userService.CreateAsync(model);

        return Ok(ApiResponse<LoginResponseModel>.Success(
            StatusCodes.Status200OK,
            result,
            "Đăng ký thành công"
        ));
    }
}
