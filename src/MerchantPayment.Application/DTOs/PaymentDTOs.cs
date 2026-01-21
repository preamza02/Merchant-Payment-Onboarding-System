using System.ComponentModel.DataAnnotations;

namespace MerchantPayment.Application.DTOs;

public class CreatePaymentRequest
{
    [Required(ErrorMessage = "Merchant ID is required")]
    public Guid MerchantId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
    public decimal Amount { get; set; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3 characters")]
    public string Currency { get; set; } = "USD";

    public string? IdempotencyKey { get; set; }
}

public class PaymentCallbackRequest
{
    [Required(ErrorMessage = "Transaction ID is required")]
    public Guid TransactionId { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Success|Failed)$", ErrorMessage = "Status must be Success or Failed")]
    public string Status { get; set; } = string.Empty;

    public string? ExternalReferenceId { get; set; }
    public string? Message { get; set; }
}

public class PaymentResponse
{
    public Guid TransactionId { get; set; }
    public Guid MerchantId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
