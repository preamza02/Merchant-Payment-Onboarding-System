using MerchantPayment.Application.DTOs;

namespace MerchantPayment.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> GetByIdAsync(Guid transactionId);
    Task<IEnumerable<PaymentResponse>> GetByMerchantIdAsync(Guid merchantId);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse> ProcessCallbackAsync(PaymentCallbackRequest request);
}
