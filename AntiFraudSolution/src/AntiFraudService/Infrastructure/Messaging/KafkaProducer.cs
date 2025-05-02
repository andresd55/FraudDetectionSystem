using Confluent.Kafka;
using System.Text.Json;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Infrastructure.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(IConfiguration config)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _topic = config["Kafka:TransactionValidatedTopic"];
        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
    }

    public async Task PublishValidationResultAsync(TransactionValidation validation)
    {
        var message = new Message<string, string>
        {
            Key = validation.TransactionId.ToString(),
            Value = JsonSerializer.Serialize(new
            {
                transactionId = validation.TransactionId,
                status = validation.Status
            })
        };

        await _producer.ProduceAsync(_topic, message);
        Console.WriteLine($"📤 Publicado evento de validación: {validation.TransactionId} - {validation.Status}");
    }
}
