using System.Text.Json;
using Confluent.Kafka;
using TransactionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Infrastructure.Messaging;

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

        var config = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = "transaction-update-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_config["Kafka:TransactionValidatedTopic"]);

        _logger.LogInformation("Kafka consumer for 'transaction-validated' topic started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                _logger.LogInformation("Received Kafka event: {payload}", result.Message.Value);
                var message = JsonSerializer.Deserialize<TransactionValidatedEvent>(result.Message.Value);

                if (message == null)
                {
                    _logger.LogWarning("Could not deserialize transaction-validated event");
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                _logger.LogInformation("Looking for transaction with ID {Id}", message.TransactionId);

                var transaction = await db.Transactions
                    .FirstOrDefaultAsync(t => t.Id == message.TransactionId, stoppingToken);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction with ID {Id} not found", message.TransactionId);
                    continue;
                }

                transaction.Status = message.Status;
                _logger.LogInformation("Updating status to {Status}", message.Status);
                db.Entry(transaction).Property(t => t.Status).IsModified = true;
                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Transaction {Id} successfully updated to {Status}", message.TransactionId, message.Status);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Error while consuming Kafka message");
            }
        }
    }
}
