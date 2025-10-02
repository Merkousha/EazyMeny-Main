using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Handler برای ایجاد سفارش جدید
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IRepository<Order> orderRepository,
        IRepository<Restaurant> restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // دریافت اطلاعات رستوران
        var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new Exception("رستوران یافت نشد.");
        }

        // محاسبه مبالغ
        decimal subTotal = request.Items.Sum(item => 
            (item.DiscountedPrice ?? item.UnitPrice) * item.Quantity);
        
        decimal discount = request.Items.Sum(item => 
            item.DiscountedPrice.HasValue 
                ? (item.UnitPrice - item.DiscountedPrice.Value) * item.Quantity 
                : 0);

        decimal deliveryFee = request.IsTakeaway ? 0m : restaurant.DeliveryFee;
        decimal totalAmount = subTotal + deliveryFee - discount;

        // تولید شماره سفارش یکتا
        string orderNumber = GenerateOrderNumber();

        // ایجاد سفارش
        var order = new Order
        {
            OrderNumber = orderNumber,
            RestaurantId = request.RestaurantId,
            
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            DeliveryAddress = request.IsTakeaway ? null : request.DeliveryAddress,
            
            SubTotal = subTotal,
            Discount = discount,
            DeliveryFee = deliveryFee,
            TotalAmount = totalAmount,
            
            OrderDate = DateTime.Now,
            DesiredDeliveryTime = request.PreferredDeliveryTime,
            Status = OrderStatus.Pending,
            IsPaid = false,
            IsDelivery = !request.IsTakeaway,
            CustomerNotes = request.Note
        };

        // ایجاد آیتم‌های سفارش
        foreach (var item in request.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.DiscountedPrice ?? item.UnitPrice, // قیمت نهایی
                TotalPrice = (item.DiscountedPrice ?? item.UnitPrice) * item.Quantity
            };

            order.OrderItems.Add(orderItem);
        }

        // ذخیره سفارش
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    /// <summary>
    /// تولید شماره سفارش یکتا (فرمت: ORD-YYYYMMDD-XXXXX)
    /// </summary>
    private string GenerateOrderNumber()
    {
        var datePart = DateTime.Now.ToString("yyyyMMdd");
        var randomPart = new Random().Next(10000, 99999);
        return $"ORD-{datePart}-{randomPart}";
    }
}
