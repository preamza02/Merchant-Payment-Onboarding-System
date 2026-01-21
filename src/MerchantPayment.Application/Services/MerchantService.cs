using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Domain.Enums;

namespace MerchantPayment.Application.Services;

public class MerchantService : IMerchantService
{
    private readonly IMerchantRepository _merchantRepository;

    public MerchantService(IMerchantRepository merchantRepository)
    {
        _merchantRepository = merchantRepository;
    }

    public async Task<MerchantResponse> GetByIdAsync(Guid merchantId)
    {
        var merchant = await _merchantRepository.GetByIdAsync(merchantId)
            ?? throw new KeyNotFoundException($"Merchant with ID {merchantId} not found");

        return MapToResponse(merchant);
    }

    public async Task<IEnumerable<MerchantResponse>> GetAllAsync()
    {
        var merchants = await _merchantRepository.GetAllAsync();
        return merchants.Select(MapToResponse);
    }

    public async Task<MerchantResponse> CreateAsync(CreateMerchantRequest request)
    {
        var existingMerchant = await _merchantRepository.GetByEmailAsync(request.Email);
        if (existingMerchant != null)
        {
            throw new InvalidOperationException($"Merchant with email {request.Email} already exists");
        }

        var merchant = new Merchant
        {
            MerchantId = Guid.NewGuid(),
            BusinessName = request.BusinessName,
            Email = request.Email,
            Status = MerchantStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdMerchant = await _merchantRepository.CreateAsync(merchant);
        return MapToResponse(createdMerchant);
    }

    public async Task<MerchantResponse> UpdateAsync(Guid merchantId, UpdateMerchantRequest request)
    {
        var merchant = await _merchantRepository.GetByIdAsync(merchantId)
            ?? throw new KeyNotFoundException($"Merchant with ID {merchantId} not found");

        if (!string.IsNullOrEmpty(request.BusinessName))
            merchant.BusinessName = request.BusinessName;

        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingMerchant = await _merchantRepository.GetByEmailAsync(request.Email);
            if (existingMerchant != null && existingMerchant.MerchantId != merchantId)
            {
                throw new InvalidOperationException($"Email {request.Email} is already in use");
            }
            merchant.Email = request.Email;
        }

        merchant.UpdatedAt = DateTime.UtcNow;
        var updatedMerchant = await _merchantRepository.UpdateAsync(merchant);
        return MapToResponse(updatedMerchant);
    }

    public async Task<MerchantResponse> UpdateStatusAsync(Guid merchantId, UpdateMerchantStatusRequest request)
    {
        var merchant = await _merchantRepository.GetByIdAsync(merchantId)
            ?? throw new KeyNotFoundException($"Merchant with ID {merchantId} not found");

        merchant.Status = Enum.Parse<MerchantStatus>(request.Status);
        merchant.UpdatedAt = DateTime.UtcNow;

        var updatedMerchant = await _merchantRepository.UpdateAsync(merchant);
        return MapToResponse(updatedMerchant);
    }

    public async Task<bool> DeleteAsync(Guid merchantId)
    {
        if (!await _merchantRepository.ExistsAsync(merchantId))
        {
            throw new KeyNotFoundException($"Merchant with ID {merchantId} not found");
        }

        return await _merchantRepository.DeleteAsync(merchantId);
    }

    private static MerchantResponse MapToResponse(Merchant merchant)
    {
        return new MerchantResponse
        {
            MerchantId = merchant.MerchantId,
            BusinessName = merchant.BusinessName,
            Email = merchant.Email,
            Status = merchant.Status.ToString(),
            CreatedAt = merchant.CreatedAt,
            UpdatedAt = merchant.UpdatedAt
        };
    }
}
