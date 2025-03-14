using CollabSphere.Common;
using CollabSphere.Modules.Auth.Models;
using CollabSphere.Modules.Auth.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.Auth;

public class AuthController : ApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginUserModel loginUserModel)
    {
        var result = await _authService.LoginAsync(loginUserModel);

        return Ok(ApiResponse<LoginResponseModel>.Success(
            StatusCodes.Status200OK,
            result,
            "Đăng nhập thành công"
        ));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Xóa cookie ASP.NET Core Identity
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            // Secure = true, // Chỉ nên true nếu bạn dùng HTTPS
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Ok(ApiResponse<BaseResponseModel>.Success(
            StatusCodes.Status200OK,
            new BaseResponseModel { },
            "Đăng xuất thành công"
        ));
    }

    [HttpPut("{id:guid}/changePassword")]
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordModel changePasswordModel)
    {
        return Ok(ApiResponse<BaseResponseModel>.Success(StatusCodes.Status200OK,
            await _authService.ChangePasswordAsync(id, changePasswordModel),
            ""
            ));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserModel model)
    {
        var result = await _authService.CreateAsync(model);

        return Ok(ApiResponse<LoginResponseModel>.Success(
            StatusCodes.Status200OK,
            result,
            "Đăng ký thành công"
        ));
    }

    [HttpGet("email-verification/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmailAsync([FromRoute] string token)
    {
        _authService.VerifyEmailAsync(token);

        return Ok(ApiResponse<BaseResponseModel>.Success(
            StatusCodes.Status200OK,
            new BaseResponseModel { },
            "Email của bạn đã được xác thực"
        ));
    }
}
