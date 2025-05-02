namespace TransactionService.Infrastructure.Messaging;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string TransactionCreatedTopic { get; set; } = "transaction-created";
}
