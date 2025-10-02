using EazyMenu.Application.Common.Models.Order;
using EazyMenu.Domain.Enums;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetAllOrders;

/// <summary>
/// Query برای دریافت لیست سفارش‌ها با فیلترهای پیشرفته
/// </summary>
public record GetAllOrdersQuery(
    Guid? RestaurantId = null,
    OrderStatus? Status = null,
    bool? IsPaid = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<List<OrderListDto>>;
