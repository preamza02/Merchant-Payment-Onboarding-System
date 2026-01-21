using MerchantPayment.Application.DTOs;

namespace MerchantPayment.Application.Interfaces;

public interface IMerchantService
{
    Task<MerchantResponse> GetByIdAsync(Guid merchantId);
    Task<IEnumerable<MerchantResponse>> GetAllAsync();
    Task<MerchantResponse> CreateAsync(CreateMerchantRequest request);
    Task<MerchantResponse> UpdateAsync(Guid merchantId, UpdateMerchantRequest request);
    Task<MerchantResponse> UpdateStatusAsync(Guid merchantId, UpdateMerchantStatusRequest request);
    Task<bool> DeleteAsync(Guid merchantId);
}
