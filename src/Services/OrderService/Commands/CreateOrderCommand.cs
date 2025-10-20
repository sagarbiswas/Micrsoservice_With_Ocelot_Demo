using Common.Commands;
using Common.Models;
using MediatR;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Commands;

public record CreateOrderCommand(string CustomerName, List<OrderItemDto> Items) : ICommand<Guid>;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal Price);

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly OrderDbContext _context;

    public CreateOrderCommandHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = request.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(order.Id);
    }
}