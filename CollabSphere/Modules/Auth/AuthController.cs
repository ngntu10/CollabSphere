using CollabSphere.Common;
using CollabSphere.Modules.Auth.Models;
using CollabSphere.Modules.Auth.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("sessionToken", new CookieOptions
        {
            Path = "/", // Đảm bảo trùng với Path của cookie lúc tạo
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.None
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

    [HttpPost("email-verification/{token}")]
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

    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuthStatusAsync()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();

            if (user == null)
            {
                return Ok(ApiResponse<object>.Success(
                    StatusCodes.Status200OK,
                    new { isAuthenticated = false },
                    "Người dùng chưa đăng nhập"
                ));
            }

            var accountResponse = new AccountResponse(
                user.Id.ToString(),
                user.UserName,
                user.Email
            );

            return Ok(ApiResponse<object>.Success(
                StatusCodes.Status200OK,
                new
                {
                    isAuthenticated = true,
                    account = accountResponse
                },
                "Người dùng đã đăng nhập"
            ));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<object>.Success(
                StatusCodes.Status200OK,
                new { isAuthenticated = false },
                "Đã xảy ra lỗi khi kiểm tra trạng thái đăng nhập"
            ));
        }
    }
}
