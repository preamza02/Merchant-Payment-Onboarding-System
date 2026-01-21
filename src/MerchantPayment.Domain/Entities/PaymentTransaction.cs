using MerchantPayment.Domain.Enums;

namespace MerchantPayment.Domain.Entities;

public class PaymentTransaction
{
    public Guid TransactionId { get; set; }

    public Guid MerchantId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "USD";

    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public string? ExternalReferenceId { get; set; }

    public string? IdempotencyKey { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Merchant? Merchant { get; set; }

    public virtual ICollection<TransactionAuditLog> AuditLogs { get; set; } = new List<TransactionAuditLog>();
}
