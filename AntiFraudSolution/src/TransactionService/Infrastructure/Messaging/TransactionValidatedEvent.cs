using System.Text.Json.Serialization;

namespace TransactionService.Infrastructure.Messaging;

public class TransactionValidatedEvent
{
    [JsonPropertyName("transactionId")]
    public Guid TransactionId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";
}
