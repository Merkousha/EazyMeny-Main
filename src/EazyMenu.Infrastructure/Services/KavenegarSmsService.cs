using EazyMenu.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// سرویس ارسال SMS با Kavenegar
/// </summary>
public class KavenegarSmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _sender;

    public KavenegarSmsService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["SMS:Kavenegar:ApiKey"] ?? throw new InvalidOperationException("Kavenegar ApiKey not configured");
        _sender = configuration["SMS:Kavenegar:Sender"] ?? "10008663";
        _httpClient.BaseAddress = new Uri($"https://api.kavenegar.com/v1/{_apiKey}/");
    }

    public async Task<bool> SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"sms/send.json?receptor={phoneNumber}&sender={_sender}&message={Uri.EscapeDataString(message)}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"verify/lookup.json?receptor={phoneNumber}&token={code}&template=OTP", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendByTemplateAsync(string phoneNumber, string template, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokens = string.Join("&", parameters.Select((p, i) => $"token{(i > 0 ? (i + 1).ToString() : "")}={p.Value}"));
            var response = await _httpClient.GetAsync($"verify/lookup.json?receptor={phoneNumber}&template={template}&{tokens}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
