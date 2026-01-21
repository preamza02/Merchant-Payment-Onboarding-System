using MerchantPayment.Domain.Enums;

namespace MerchantPayment.Domain.Entities;

public class Merchant
{
    public Guid MerchantId { get; set; }

    public string BusinessName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public MerchantStatus Status { get; set; } = MerchantStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}
