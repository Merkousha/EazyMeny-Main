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

    public GetOrderDetailsQueryHandler(
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Restaurant> restaurantRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _restaurantRepository = restaurantRepository;
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
            Status = GetStatusTitle(order.Status),
            SubTotal = order.SubTotal,
            DeliveryFee = order.DeliveryFee,
            Tax = order.Tax,
            Discount = order.Discount,
            TotalAmount = order.TotalAmount,
            IsPaid = order.IsPaid,
            IsOnlinePayment = order.IsOnlinePayment,
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

    /// <summary>
    /// نمایش فارسی وضعیت سفارش
    /// </summary>
    private static string GetStatusTitle(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "در انتظار تایید",
        OrderStatus.Confirmed => "تایید شده",
        OrderStatus.Preparing => "در حال آماده‌سازی",
        OrderStatus.Ready => "آماده تحویل",
        OrderStatus.Delivered => "تحویل داده شده",
        OrderStatus.Cancelled => "لغو شده",
        _ => status.ToString()
    };
}
