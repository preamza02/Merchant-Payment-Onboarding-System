using System.ComponentModel.DataAnnotations;

namespace MerchantPayment.Application.DTOs;

public class CreateMerchantRequest
{
    [Required(ErrorMessage = "Business name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Business name must be between 2 and 200 characters")]
    public string BusinessName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}

public class UpdateMerchantRequest
{
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Business name must be between 2 and 200 characters")]
    public string? BusinessName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
}

public class UpdateMerchantStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Active|Suspended|Rejected)$", ErrorMessage = "Status must be Active, Suspended, or Rejected")]
    public string Status { get; set; } = string.Empty;
}

public class MerchantResponse
{
    public Guid MerchantId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
