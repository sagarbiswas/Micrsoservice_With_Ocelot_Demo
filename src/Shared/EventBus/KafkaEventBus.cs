using Common.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventBus;

public class KafkaEventBus : IEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaEventBus> _logger;

    public KafkaEventBus(IProducer<string, string> producer, IConsumer<string, string> consumer, ILogger<KafkaEventBus> logger)
    {
        _producer = producer;
        _consumer = consumer;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event,string topic) where T : class, IIntegrationEvent
    {
        topic = string.IsNullOrEmpty(topic) ? typeof(T).Name : topic;
        var message = JsonSerializer.Serialize(@event);
        
        await _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = @event.Id.ToString(),
            Value = message
        });
        
        _logger.LogInformation("Published event {EventType} with ID {EventId}", typeof(T).Name, @event.Id);
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler, string? topic) where T : class, IIntegrationEvent
    {
         topic = string.IsNullOrEmpty(topic)? typeof(T).Name: topic;
        _consumer.Subscribe(topic);

        await Task.Run(async () =>
        {
            while (true)
            {
                var result = _consumer.Consume();
                if (result?.Message?.Value != null)
                {
                    var @event = JsonSerializer.Deserialize<T>(result.Message.Value);
                    if (@event != null)
                    {
                        await handler(@event);
                        _consumer.Commit(result);
                    }
                }
            }
        });
    }
}