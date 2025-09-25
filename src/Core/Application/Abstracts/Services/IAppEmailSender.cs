namespace Application.Abstracts.Services;

public interface IAppEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}
