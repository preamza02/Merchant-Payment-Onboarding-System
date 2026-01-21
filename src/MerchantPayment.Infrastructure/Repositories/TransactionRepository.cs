using Microsoft.EntityFrameworkCore;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Infrastructure.Data;

namespace MerchantPayment.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(Guid transactionId)
    {
        return await _context.Transactions
            .Include(t => t.AuditLogs)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<PaymentTransaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetByMerchantIdAsync(Guid merchantId)
    {
        return await _context.Transactions
            .Where(t => t.MerchantId == merchantId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<PaymentTransaction> UpdateAsync(PaymentTransaction transaction)
    {
        _context.Entry(transaction).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<PaymentTransaction> UpdateWithAuditAsync(PaymentTransaction transaction, TransactionAuditLog auditLog)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(transaction).State = EntityState.Modified;
                _context.TransactionAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return transaction;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<int> CountRecentByMerchantAsync(Guid merchantId, TimeSpan timeWindow)
    {
        var cutoffTime = DateTime.UtcNow - timeWindow;
        return await _context.Transactions
            .CountAsync(t => t.MerchantId == merchantId && t.CreatedAt >= cutoffTime);
    }
}
