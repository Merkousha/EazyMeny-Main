using EazyMenu.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس OTP با Memory Cache
/// </summary>
public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;
    private const int OtpExpirationMinutes = 2;
    private const int OtpLength = 5;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// تولید کد OTP 5 رقمی
    /// </summary>
    public Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        // تولید کد 5 رقمی تصادفی
        var random = new Random();
        var otpCode = random.Next(10000, 99999).ToString();

        // ذخیره در Cache با مدت زمان 2 دقیقه
        var cacheKey = GetCacheKey(phoneNumber);
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(OtpExpirationMinutes));

        _cache.Set(cacheKey, otpCode, cacheOptions);

        return Task.FromResult(otpCode);
    }

    /// <summary>
    /// بررسی صحت کد OTP
    /// </summary>
    public Task<bool> VerifyOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(phoneNumber);

        if (_cache.TryGetValue<string>(cacheKey, out var cachedOtp))
        {
            return Task.FromResult(cachedOtp == code);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// حذف کد OTP
    /// </summary>
    public Task RemoveOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(phoneNumber);
        _cache.Remove(cacheKey);
        return Task.CompletedTask;
    }

    private static string GetCacheKey(string phoneNumber) => $"OTP_{phoneNumber}";
}
