using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Helpers;
using CollabSphere.Modules.Auth.Models;
using CollabSphere.Modules.Email.Config;
using CollabSphere.Modules.Email.Services;
using CollabSphere.Modules.Template.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Auth.Services.Impl;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;
    private readonly ITemplateService _templateService;
    private readonly UserManager<User> _userManager;

    public AuthService(IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        ITemplateService templateService,
        IEmailService emailService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _templateService = templateService;
        _emailService = emailService;
    }

    public async Task<LoginResponseModel> CreateAsync(CreateUserModel createUserModel)
    {
        var user = _mapper.Map<User>(createUserModel);

        var result = await _userManager.CreateAsync(user, createUserModel.Password);

        if (!result.Succeeded) throw new BadRequestException(result.Errors.FirstOrDefault()?.Description);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailTemplate = await _templateService.GetTemplateAsync(TemplateConstants.ConfirmationEmail);

        string url = "http://localhost:3000/auth/email-verification" + token;

        var emailBody = _templateService.ReplaceInTemplate(emailTemplate,
            new Dictionary<string, string> { { "{UserId}", user.Id.ToString() }, { "{url}", url } });

        await _emailService.SendEmailAsync(EmailMessage.Create(user.Email, emailBody, "[CollabSphere] Confirm your email"));

        var tokenResponse = JwtHelper.GenerateToken(user, _configuration);
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
}
