using Common.Events;

namespace OrderService.Events;

public record OrderCreatedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount) : IIntegrationEvent;