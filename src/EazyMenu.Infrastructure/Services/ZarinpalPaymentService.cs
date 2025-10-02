using EazyMenu.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace EazyMenu.Infrastructure.Services;

/// <summary>
/// سرویس پرداخت با Zarinpal
/// </summary>
public class ZarinpalPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly string _merchantId;
    private readonly string _requestUrl;
    private readonly string _verifyUrl;
    private readonly string _paymentUrl;

    public ZarinpalPaymentService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _merchantId = configuration["Payment:Zarinpal:MerchantId"] ?? throw new InvalidOperationException("Zarinpal MerchantId not configured");
        _requestUrl = "https://api.zarinpal.com/pg/v4/payment/request.json";
        _verifyUrl = "https://api.zarinpal.com/pg/v4/payment/verify.json";
        _paymentUrl = "https://www.zarinpal.com/pg/StartPay/";
    }

    public async Task<PaymentRequestResult> RequestPaymentAsync(decimal amount, string description, string callbackUrl, CancellationToken cancellationToken = default)
    {
        var requestData = new
        {
            merchant_id = _merchantId,
            amount = (int)(amount * 10), // تومان به ریال
            description,
            callback_url = callbackUrl
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_requestUrl, requestData, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ZarinpalRequestResponse>(cancellationToken: cancellationToken);

            if (result?.Data?.Code == 100)
            {
                return new PaymentRequestResult
                {
                    IsSuccess = true,
                    Authority = result.Data.Authority,
                    PaymentUrl = $"{_paymentUrl}{result.Data.Authority}"
                };
            }

            return new PaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = result?.Errors?.FirstOrDefault()?.Message ?? "خطای نامشخص"
            };
        }
        catch (Exception ex)
        {
            return new PaymentRequestResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(string authority, decimal amount, CancellationToken cancellationToken = default)
    {
        var verifyData = new
        {
            merchant_id = _merchantId,
            authority,
            amount = (int)(amount * 10)
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_verifyUrl, verifyData, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ZarinpalVerifyResponse>(cancellationToken: cancellationToken);

            if (result?.Data?.Code == 100 || result?.Data?.Code == 101)
            {
                return new PaymentVerificationResult
                {
                    IsSuccess = true,
                    RefID = result.Data.RefId
                };
            }

            return new PaymentVerificationResult
            {
                IsSuccess = false,
                ErrorMessage = result?.Errors?.FirstOrDefault()?.Message ?? "تراکنش ناموفق"
            };
        }
        catch (Exception ex)
        {
            return new PaymentVerificationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    // DTOs for Zarinpal
    private class ZarinpalRequestResponse
    {
        public ZarinpalData? Data { get; set; }
        public List<ZarinpalError>? Errors { get; set; }
    }

    private class ZarinpalVerifyResponse
    {
        public ZarinpalVerifyData? Data { get; set; }
        public List<ZarinpalError>? Errors { get; set; }
    }

    private class ZarinpalData
    {
        public int Code { get; set; }
        public string Authority { get; set; } = string.Empty;
    }

    private class ZarinpalVerifyData
    {
        public int Code { get; set; }
        public long RefId { get; set; }
    }

    private class ZarinpalError
    {
        public string Message { get; set; } = string.Empty;
    }
}
