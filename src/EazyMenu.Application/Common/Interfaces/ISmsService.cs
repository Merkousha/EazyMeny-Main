namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// Interface سرویس پیامک (کاوه‌نگار)
/// </summary>
public interface ISmsService
{
    Task<bool> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task<bool> SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
    Task<bool> SendByTemplateAsync(string phoneNumber, string template, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}
