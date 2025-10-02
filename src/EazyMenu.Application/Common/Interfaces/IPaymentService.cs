namespace EazyMenu.Application.Common.Interfaces;

/// <summary>
/// Interface سرویس پرداخت (زرین‌پال)
/// </summary>
public interface IPaymentService
{
    Task<PaymentRequestResult> RequestPaymentAsync(decimal amount, string description, string callbackUrl, CancellationToken cancellationToken = default);
    Task<PaymentVerificationResult> VerifyPaymentAsync(string authority, decimal amount, CancellationToken cancellationToken = default);
}

public class PaymentRequestResult
{
    public bool IsSuccess { get; set; }
    public string? Authority { get; set; }
    public string? PaymentUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentVerificationResult
{
    public bool IsSuccess { get; set; }
    public long? RefID { get; set; }
    public string? CardPan { get; set; }
    public string? ErrorMessage { get; set; }
}
