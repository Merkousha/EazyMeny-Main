using EazyMenu.Application.Common.Models.Order;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetOrderDetails;

/// <summary>
/// Query برای دریافت جزئیات سفارش
/// </summary>
public record GetOrderDetailsQuery(Guid OrderId) : IRequest<OrderDetailsDto?>;
