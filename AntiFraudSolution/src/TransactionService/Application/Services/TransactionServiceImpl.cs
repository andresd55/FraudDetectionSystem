using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.ExternalServices;
using TransactionService.Infrastructure.Messaging;
using TransactionService.Infrastructure.Persistence;

namespace TransactionService.Application.Services;

public class TransactionServiceImpl : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly AntiFraudHttpClient _antiFraud;
    private readonly KafkaProducer _kafka;

    public TransactionServiceImpl(AppDbContext context, AntiFraudHttpClient antiFraud, KafkaProducer kafka)
    {
        _context = context;
        _antiFraud = antiFraud;
        _kafka = kafka;
    }

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest dto)
    {
        var transaction = new Transaction
        {
            SourceAccountId = dto.SourceAccountId,
            TargetAccountId = dto.TargetAccountId,
            TransferTypeId = dto.TransferTypeId,
            Value = dto.Value,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        Console.WriteLine("Saving transaction...");
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        Console.WriteLine("Transaction saved successfully.");

        await _kafka.SendTransactionCreatedAsync(transaction);
        Console.WriteLine("TransactionCreated event published to Kafka.");
        return transaction;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }
}
