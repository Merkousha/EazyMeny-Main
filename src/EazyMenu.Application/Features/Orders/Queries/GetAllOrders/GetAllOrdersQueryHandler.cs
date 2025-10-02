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

        // فیلتر بر اساس رستوران
        if (request.RestaurantId.HasValue)
        {
            orders = orders.Where(o => o.RestaurantId == request.RestaurantId.Value);
        }

        // فیلتر بر اساس وضعیت
        if (request.Status.HasValue)
        {
            orders = orders.Where(o => o.Status == request.Status.Value);
        }

        // فیلتر بر اساس وضعیت پرداخت
        if (request.IsPaid.HasValue)
        {
            orders = orders.Where(o => o.IsPaid == request.IsPaid.Value);
        }

        // فیلتر بر اساس تاریخ (از)
        if (request.FromDate.HasValue)
        {
            orders = orders.Where(o => o.OrderDate >= request.FromDate.Value);
        }

        // فیلتر بر اساس تاریخ (تا)
        if (request.ToDate.HasValue)
        {
            orders = orders.Where(o => o.OrderDate <= request.ToDate.Value);
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
                Status = order.Status, // استفاده از Enum مستقیماً
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                IsDelivery = order.IsDelivery,
                TableNumber = order.TableNumber,
                IsPaid = order.IsPaid,
                IsOnlinePayment = order.IsOnlinePayment,
                ItemsCount = order.OrderItems?.Count ?? 0,
                DesiredDeliveryTime = order.DesiredDeliveryTime
            })
            .ToList();

        return result;
    }
}
