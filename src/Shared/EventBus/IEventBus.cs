using Common.Events;

namespace EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, string topic) where T : class, IIntegrationEvent;
    Task SubscribeAsync<T>(Func<T, Task> handler, string topic) where T : class, IIntegrationEvent;
}