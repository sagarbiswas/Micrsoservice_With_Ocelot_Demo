using Common.Commands;
using Common.Events;
using Common.Models;
using EventBus;
using MediatR;
using OrderService.Data;
using OrderService.Events;
using OrderService.Models;

namespace OrderService.Commands;

public record CreateOrderCommand(string CustomerName, List<OrderItemDto> Items) : ICommand<Guid>;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal Price);

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly OrderDbContext _context;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(OrderDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
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

        // send notification 
        try
        {
            await CreateNotification(order);
        }
        catch (Exception e) // May be kafka not working.
        {
          Console.WriteLine(e);
        }
      
        return Result.Success(order.Id);
    }

    private async Task CreateNotification(Order order)
    {
        var orderEvent = new Events.OrderCreatedEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            order.Id,
            order.CustomerName,
            order.TotalAmount
        );
        await _eventBus.PublishAsync<OrderCreatedEvent>(orderEvent, "order-topic");
    }
}
