using EazyMenu.Application.Common.Models.Order;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetOrderById;

/// <summary>
/// Query برای دریافت جزئیات سفارش
/// </summary>
public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; set; }
}
