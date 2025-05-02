using AntiFraudService.Domain.Entities;
using AntiFraudService.Infrastructure.Messaging;
using AntiFraudService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AntiFraudService.Application.Services;

public class FraudDetectionService
{
    private readonly AppDbContext _context;
    private readonly KafkaProducer _kafka;

    public FraudDetectionService(AppDbContext context, KafkaProducer kafka)
    {
        _context = context;
        _kafka = kafka;
    }

    public async Task<TransactionValidation> EvaluateAsync(TransactionValidation validation)
    {
        validation.Status = "approved";
        validation.CreatedAt = DateTime.UtcNow;

        if (validation.Value > 2000)
        {
            validation.Status = "rejected";
        }
        else
        {
            var today = validation.CreatedAt.Date;
            var totalToday = await _context.Validations
                .Where(v => v.SourceAccountId == validation.SourceAccountId && v.CreatedAt.Date == today)
                .SumAsync(v => v.Value);

            if (totalToday + validation.Value > 20000)
            {
                validation.Status = "rejected";
            }
        }

        _context.Validations.Add(validation);
        await _context.SaveChangesAsync();
        await _kafka.PublishValidationResultAsync(validation);

        return validation;
    }
}
