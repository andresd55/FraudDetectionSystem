using Confluent.Kafka;

using System.Text.Json;

using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;

    public KafkaProducer(IConfiguration config)
    {
        _settings = config.GetSection("Kafka").Get<KafkaSettings>()!;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task SendTransactionCreatedAsync(Transaction transaction)
    {
        var message = new Message<string, string>
        {
            Key = transaction.Id.ToString(),
            Value = JsonSerializer.Serialize(transaction)
        };

        await _producer.ProduceAsync(_settings.TransactionCreatedTopic, message);
    }
}
