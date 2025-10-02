using AutoMapper;
using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Order;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Orders.Queries.GetOrderById;

/// <summary>
/// Handler برای دریافت جزئیات سفارش
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(
        IRepository<Order> orderRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        
        if (order == null)
            return null;

        return _mapper.Map<OrderDto>(order);
    }
}
