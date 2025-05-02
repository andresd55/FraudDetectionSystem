using System.Text.Json;
using Confluent.Kafka;
using AntiFraudService.Application.Services;
using AntiFraudService.Domain.Entities;
using AntiFraudService.Application.DTOs;

namespace AntiFraudService.Infrastructure.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = "antifraud-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(_config["Kafka:TransactionCreatedTopic"]);

        _logger.LogInformation("Kafka Consumer running...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var transaction = JsonSerializer.Deserialize<TransactionCreatedEvent>(result.Message.Value);
                _logger.LogInformation("Received event: TransactionId = {Id}", transaction.Id);

                if (transaction != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var fraudService = scope.ServiceProvider.GetRequiredService<FraudDetectionService>();

                    var validation = new TransactionValidation
                    {
                        TransactionId = transaction.Id,
                        SourceAccountId = transaction.SourceAccountId,
                        Value = transaction.Value
                    };

                    await fraudService.EvaluateAsync(validation);
                    _logger.LogInformation("Processed transaction: {Id}", transaction.Id);
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consumption error");
            }
        }
    }
}
