using EventBus;
using MediatR;
using OrderService.Events;

namespace OrderService.Notifications
{
    public class OrderCreatedNotificationHandler : INotificationHandler<OrderCreatedNotification>
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderCreatedNotificationHandler> _logger;
        public OrderCreatedNotificationHandler(IEventBus eventBus, ILogger<OrderCreatedNotificationHandler>  logger )
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
        {
            var orderEvent = new OrderCreatedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                notification.OrderId,
                notification.CustomerName,
                notification.TotalAmount
            );

            try
            {
                _logger.LogInformation($"OrderCreatedNotificationHandler {notification.OrderId} send successfull. Dispatching OrderCreatedNotification (fire-and-forget) from OrderCreatedNotificationHandler at {DateTime.Now}");
                 await _eventBus.PublishAsync(orderEvent, "order-topic");
                //await Task.Delay(50000);
                _logger.LogInformation($"OrderCreatedNotificationHandler {notification.OrderId} send completed. Dispatching OrderCreatedNotification (fire-and-forget) from OrderCreatedNotificationHandler at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                // Log the failure, but don't throw
                Console.WriteLine($"Failed to publish order event: {ex.Message}");
            }
        }
    }
}
