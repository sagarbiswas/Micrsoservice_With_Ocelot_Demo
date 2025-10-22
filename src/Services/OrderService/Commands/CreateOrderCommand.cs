using Common.Commands;
using Common.Events;
using Common.Models;
using EventBus;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using OrderService.Data;
using OrderService.Events;
using OrderService.Models;
using OrderService.Notifications;

namespace OrderService.Commands;

public record CreateOrderCommand(string CustomerName, List<OrderItemDto> Items) : ICommand<Guid>;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal Price);

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly OrderDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    public CreateOrderCommandHandler(OrderDbContext context, IEventBus eventBus, IMediator mediator, ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _mediator = mediator;
        _logger = logger;

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
        /*
        // send notification 
        try
        {
            await CreateNotification(order);
        }
        catch (Exception e) // May be kafka not working.
        {
          Console.WriteLine(e);
        }
       */
        //
        // Log that notification is fire-and-forget
        _logger.LogInformation($"Order {order.Id} created started. Dispatching OrderCreatedNotification (fire-and-forget) from CreateOrderCommandHandler at - {DateTime.Now}.");
        // Better way to do it using INotiifation (fire-and-forget)
        await _mediator.Publish(new OrderCreatedNotification(order.Id, order.CustomerName, order.TotalAmount), cancellationToken);
        _logger.LogInformation($"Order {order.Id} created ended. Dispatching OrderCreatedNotification (fire-and-forget) from CreateOrderCommandHandler at - {DateTime.Now}.");

        return Result.Success(order.Id);
    }

   /* private async Task CreateNotification(Order order)
    {
        var orderEvent = new Events.OrderCreatedEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            order.Id,
            order.CustomerName,
            order.TotalAmount
        );
        await _eventBus.PublishAsync<OrderCreatedEvent>(orderEvent, "order-topic");
    }*/
}
