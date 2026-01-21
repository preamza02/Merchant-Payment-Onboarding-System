using MerchantPayment.Domain.Entities;

namespace MerchantPayment.Application.Interfaces;

public interface ITransactionRepository
{
    Task<PaymentTransaction?> GetByIdAsync(Guid transactionId);
    Task<PaymentTransaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<IEnumerable<PaymentTransaction>> GetByMerchantIdAsync(Guid merchantId);
    Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction);
    Task<PaymentTransaction> UpdateAsync(PaymentTransaction transaction);
    Task<PaymentTransaction> UpdateWithAuditAsync(PaymentTransaction transaction, TransactionAuditLog auditLog);
    Task<int> CountRecentByMerchantAsync(Guid merchantId, TimeSpan timeWindow);
}
