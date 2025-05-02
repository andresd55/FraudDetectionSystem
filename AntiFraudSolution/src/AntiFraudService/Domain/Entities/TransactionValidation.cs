namespace AntiFraudService.Domain.Entities;

public class TransactionValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TransactionId { get; set; }
    public Guid SourceAccountId { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
