using CollabSphere.Modules.Email.Config;

namespace CollabSphere.Modules.Email.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}
