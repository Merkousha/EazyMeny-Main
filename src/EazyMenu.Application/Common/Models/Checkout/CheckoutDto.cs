namespace EazyMenu.Application.Common.Models.Checkout;

/// <summary>
/// DTO برای اطلاعات فرم Checkout
/// </summary>
public class CheckoutDto
{
    /// <summary>
    /// نام مشتری
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// شماره تماس مشتری
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;
    
    /// <summary>
    /// آدرس تحویل
    /// </summary>
    public string DeliveryAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// یادداشت سفارش (اختیاری)
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// زمان ترجیحی تحویل (اختیاری)
    /// </summary>
    public DateTime? PreferredDeliveryTime { get; set; }
    
    /// <summary>
    /// آیا سفارش حضوری است؟ (در غیر اینصورت پیک)
    /// </summary>
    public bool IsTakeaway { get; set; } = false;
    
    /// <summary>
    /// ID رستوران
    /// </summary>
    public Guid RestaurantId { get; set; }
    
    /// <summary>
    /// نام رستوران
    /// </summary>
    public string RestaurantName { get; set; } = string.Empty;
    
    /// <summary>
    /// مبلغ کل سفارش (قبل از هزینه ارسال)
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// هزینه ارسال
    /// </summary>
    public decimal DeliveryFee { get; set; }
    
    /// <summary>
    /// مبلغ نهایی (شامل هزینه ارسال)
    /// </summary>
    public decimal FinalAmount => TotalAmount + (IsTakeaway ? 0 : DeliveryFee);
    
    /// <summary>
    /// تخفیف کل
    /// </summary>
    public decimal TotalDiscount { get; set; }
}
