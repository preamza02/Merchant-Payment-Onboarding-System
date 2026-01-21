using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;

namespace MerchantPayment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetById(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        return Ok(ApiResponse<PaymentResponse>.SuccessResponse(payment));
    }

    [HttpGet("merchant/{merchantId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponse>>>> GetByMerchantId(Guid merchantId)
    {
        var payments = await _paymentService.GetByMerchantIdAsync(merchantId);
        return Ok(ApiResponse<IEnumerable<PaymentResponse>>.SuccessResponse(payments));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Create([FromBody] CreatePaymentRequest request)
    {
        var payment = await _paymentService.CreatePaymentAsync(request);
        return CreatedAtAction(
            nameof(GetById),
            new { id = payment.TransactionId },
            ApiResponse<PaymentResponse>.SuccessResponse(payment, "Payment initiated successfully"));
    }

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> Callback([FromBody] PaymentCallbackRequest request)
    {
        var payment = await _paymentService.ProcessCallbackAsync(request);
        return Ok(ApiResponse<PaymentResponse>.SuccessResponse(payment, "Payment callback processed successfully"));
    }
}
