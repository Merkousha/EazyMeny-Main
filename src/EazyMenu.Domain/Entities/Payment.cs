using EazyMenu.Domain.Common;

namespace EazyMenu.Domain.Entities;

/// <summary>
/// Entity پرداخت
/// </summary>
public class Payment : BaseEntity
{
    public string TransactionId { get; set; } = string.Empty; // شناسه تراکنش از زرین‌پال
    public string Authority { get; set; } = string.Empty; // Authority زرین‌پال
    
    public decimal Amount { get; set; }
    
    // نوع پرداخت
    public bool IsSubscriptionPayment { get; set; } // true: اشتراک, false: سفارش
    
    public Guid? OrderId { get; set; }
    public virtual Order? Order { get; set; }
    
    public Guid? SubscriptionId { get; set; }
    public virtual Subscription? Subscription { get; set; }
    
    // وضعیت
    public bool IsSuccessful { get; set; } = false;
    public DateTime? PaidAt { get; set; }
    
    // اطلاعات تراکنش
    public string? RefID { get; set; } // شماره پیگیری
    public string? CardPan { get; set; } // 4 رقم آخر کارت
    
    // خطا
    public string? ErrorMessage { get; set; }
    
    // Invoice
    public string? InvoiceNumber { get; set; }
}
