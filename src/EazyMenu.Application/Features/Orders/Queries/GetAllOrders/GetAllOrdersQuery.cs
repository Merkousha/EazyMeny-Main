using EazyMenu.Application.Common.Models.Order;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetAllOrders;

/// <summary>
/// Query برای دریافت لیست سفارش‌ها
/// </summary>
public class GetAllOrdersQuery : IRequest<List<OrderListDto>>
{
    public Guid? RestaurantId { get; init; }
    public string? Status { get; init; }
}
