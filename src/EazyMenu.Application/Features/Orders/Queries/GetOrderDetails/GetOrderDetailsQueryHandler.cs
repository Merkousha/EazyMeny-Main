using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Order;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetOrderDetails;

/// <summary>
/// Handler برای دریافت جزئیات سفارش
/// </summary>
public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailsDto?>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Payment> _paymentRepository;

    public GetOrderDetailsQueryHandler(
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Restaurant> restaurantRepository,
        IRepository<Payment> paymentRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _restaurantRepository = restaurantRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<OrderDetailsDto?> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var restaurant = await _restaurantRepository.GetByIdAsync(order.RestaurantId, cancellationToken);
        var orderItems = await _orderItemRepository.FindAsync(oi => oi.OrderId == order.Id, cancellationToken);
        
        // دریافت اطلاعات پرداخت (اگر وجود داشته باشد)
        Payment? payment = null;
        if (order.PaymentId.HasValue)
        {
            payment = await _paymentRepository.GetByIdAsync(order.PaymentId.Value, cancellationToken);
        }

        var dto = new OrderDetailsDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            RestaurantId = order.RestaurantId,
            RestaurantName = restaurant?.Name ?? "-",
            CustomerName = string.IsNullOrWhiteSpace(order.CustomerName) ? "مشتری مهمان" : order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            IsDelivery = order.IsDelivery,
            DeliveryAddress = order.DeliveryAddress,
            TableNumber = order.TableNumber,
            OrderDate = order.OrderDate,
            DesiredDeliveryTime = order.DesiredDeliveryTime,
            PreparedAt = order.PreparedAt,
            DeliveredAt = order.DeliveredAt,
            Status = order.Status, // استفاده از Enum مستقیماً
            SubTotal = order.SubTotal,
            DeliveryFee = order.DeliveryFee,
            Tax = order.Tax,
            Discount = order.Discount,
            TotalAmount = order.TotalAmount,
            IsPaid = order.IsPaid,
            IsOnlinePayment = order.IsOnlinePayment,
            PaymentRefID = payment?.RefID,
            CustomerNotes = order.CustomerNotes,
            CancellationReason = order.CancellationReason
        };

        dto.Items = orderItems
            .OrderBy(item => item.CreatedAt)
            .Select(item => new OrderItemDto
            {
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                SelectedOptions = item.SelectedOptions,
                SpecialInstructions = item.SpecialInstructions
            })
            .ToList();

        return dto;
    }
}
