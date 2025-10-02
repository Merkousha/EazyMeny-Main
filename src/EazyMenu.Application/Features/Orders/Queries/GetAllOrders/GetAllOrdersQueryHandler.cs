using System;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Order;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetAllOrders;

/// <summary>
/// Handler برای دریافت لیست سفارش‌ها
/// </summary>
public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderListDto>>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public GetAllOrdersQueryHandler(IRepository<Order> orderRepository, IRepository<Restaurant> restaurantRepository)
    {
        _orderRepository = orderRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<List<OrderListDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        if (request.RestaurantId.HasValue)
        {
            orders = orders.Where(o => o.RestaurantId == request.RestaurantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<OrderStatus>(request.Status, true, out var statusFilter))
        {
            orders = orders.Where(o => o.Status == statusFilter);
        }

        // کش کردن نام رستوران‌ها برای کاهش فراخوانی
        var restaurantIds = orders.Select(o => o.RestaurantId).Distinct().ToList();
        var restaurants = await _restaurantRepository.FindAsync(r => restaurantIds.Contains(r.Id), cancellationToken);
        var restaurantLookup = restaurants.ToDictionary(r => r.Id, r => r.Name);

        var result = orders
            .OrderByDescending(o => o.CreatedAt)
            .Select(order => new OrderListDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                RestaurantName = restaurantLookup.TryGetValue(order.RestaurantId, out var name) ? name : "-",
                CustomerName = string.IsNullOrWhiteSpace(order.CustomerName) ? "مشتری مهمان" : order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                Status = GetStatusTitle(order.Status),
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                IsDelivery = order.IsDelivery,
                TableNumber = order.TableNumber,
                IsPaid = order.IsPaid,
                IsOnlinePayment = order.IsOnlinePayment
            })
            .ToList();

        return result;
    }

    /// <summary>
    /// نمایش فارسی برای وضعیت سفارش
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
