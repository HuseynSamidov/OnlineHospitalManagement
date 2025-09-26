namespace Application.Abstracts.Services;

public interface IAppEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
