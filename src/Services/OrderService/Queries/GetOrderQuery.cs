using Common.Models;
using Common.Queries;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Queries;

public record GetOrderQuery(Guid OrderId) : IQuery<Order>;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, Order>
{
    private readonly OrderDbContext _context;

    public GetOrderQueryHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        return order != null 
            ? Result.Success(order) 
            : Result.Failure<Order>("Order not found");
    }
}