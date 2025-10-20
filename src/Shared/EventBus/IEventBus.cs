using Common.Events;

namespace EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class, IIntegrationEvent;
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : class, IIntegrationEvent;
}