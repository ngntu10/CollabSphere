using System;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Helpers;
using CollabSphere.Modules.Auth.Models;
using CollabSphere.Modules.Email.Config;
using CollabSphere.Modules.Email.Services;
using CollabSphere.Modules.Template.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CollabSphere.Modules.Auth.Services.Impl;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IEmailVerificationTokenService _emailVerificationTokenService;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;
    private readonly ITemplateService _templateService;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthService(IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        ITemplateService templateService,
        IEmailService emailService,
        IEmailVerificationTokenService emailVerificationTokenService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _templateService = templateService;
        _emailService = emailService;
        _emailVerificationTokenService = emailVerificationTokenService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<LoginResponseModel> CreateAsync(CreateUserModel createUserModel)
    {
        var user = _mapper.Map<User>(createUserModel);

        var result = await _userManager.CreateAsync(user, createUserModel.Password);

        if (!result.Succeeded) throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        var emailVerificationToken = _emailVerificationTokenService.CreateEmailTokenAsync(user).Result;
        // var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var token = emailVerificationToken.Token;
        var emailTemplate = await _templateService.GetTemplateAsync(TemplateConstants.ConfirmationEmail);

        string url = "http://localhost:3000/auth/email-verification/" + token;

        var emailBody = _templateService.ReplaceInTemplate(emailTemplate,
            new Dictionary<string, string> { { "{UserId}", user.Id.ToString() }, { "{url}", url } });

        await _emailService.SendEmailAsync(EmailMessage.Create(user.Email, emailBody, "[CollabSphere] Confirm your email"));

        var tokenResponse = JwtHelper.GenerateToken(user, _configuration);

        _httpContextAccessor.HttpContext.Response.Cookies.Append("access_token", tokenResponse, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new LoginResponseModel()
        {
            Token = tokenResponse,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            account = new AccountResponse(user.Id.ToString(), user.UserName, user.Email)
        };
    }

    public async Task<LoginResponseModel> LoginAsync(LoginUserModel loginUserModel)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginUserModel.Username);

        if (user == null)
            throw new NotFoundException("Username or password is incorrect");

        var previousLoginDate = user.LastLoginDate;

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginUserModel.Password, false, false);

        if (!signInResult.Succeeded)
            throw new BadRequestException("Username or password is incorrect");

        user.LastLoginDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var tokenResponse = JwtHelper.GenerateToken(user, _configuration);

        _httpContextAccessor.HttpContext.Response.Cookies.Append("sessionToken", tokenResponse, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new LoginResponseModel
        {
            Token = tokenResponse,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            account = new AccountResponse(user.Id.ToString(), user.UserName, user.Email),
            LastLoginDate = previousLoginDate
        };
    }

    public async Task<BaseResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            throw new NotFoundException("Auth does not exist anymore");

        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword,
                changePasswordModel.NewPassword);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        return new BaseResponseModel
        {
            Id = user.Id,
        };
    }

    public async Task VerifyEmailAsync(string token)
    {
        _logger.LogInformation("Đang xác thực email với token: {Token}", token);

        // Sử dụng một DbContext riêng cho cả quy trình này
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Tìm token và user trong một truy vấn
                var tokenEntity = await dbContext.EmailVerificationTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Token == token);

                if (tokenEntity == null || tokenEntity.User == null)
                    throw new NotFoundException("Token không hợp lệ hoặc đã hết hạn");

                var user = tokenEntity.User;
                user.EmailConfirmed = true;

                await userManager.UpdateAsync(user);
                dbContext.EmailVerificationTokens.Remove(tokenEntity);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Email đã được xác thực thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Lỗi khi xác thực email: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }

    public async Task<User> GetCurrentUserAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.Request.Cookies.TryGetValue("sessionToken", out var token) || string.IsNullOrEmpty(token))
            {
                return null;
            }

            var principal = JwtHelper.ValidateToken(token, _configuration);
            if (principal == null)
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng hiện tại");
            return null;
        }
    }
}
