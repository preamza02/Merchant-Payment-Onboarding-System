using MerchantPayment.Domain.Entities;

namespace MerchantPayment.Application.Interfaces;

public interface IMerchantRepository
{
    Task<Merchant?> GetByIdAsync(Guid merchantId);
    Task<Merchant?> GetByEmailAsync(string email);
    Task<IEnumerable<Merchant>> GetAllAsync();
    Task<Merchant> CreateAsync(Merchant merchant);
    Task<Merchant> UpdateAsync(Merchant merchant);
    Task<bool> DeleteAsync(Guid merchantId);
    Task<bool> ExistsAsync(Guid merchantId);
}
