using TransactionService.Domain.Entities;
using TransactionService.Application.DTOs;

namespace TransactionService.Application.Interfaces;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest dto);
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
}
