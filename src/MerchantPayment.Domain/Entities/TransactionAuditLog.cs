namespace MerchantPayment.Domain.Entities;

public class TransactionAuditLog
{
    public Guid AuditLogId { get; set; }

    public Guid TransactionId { get; set; }

    public string PreviousStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual PaymentTransaction? Transaction { get; set; }
}
