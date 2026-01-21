using Microsoft.EntityFrameworkCore;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Infrastructure.Data;

namespace MerchantPayment.Infrastructure.Repositories;

public class MerchantRepository : IMerchantRepository
{
    private readonly ApplicationDbContext _context;

    public MerchantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Merchant?> GetByIdAsync(Guid merchantId)
    {
        return await _context.Merchants.FindAsync(merchantId);
    }

    public async Task<Merchant?> GetByEmailAsync(string email)
    {
        return await _context.Merchants
            .FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Merchant>> GetAllAsync()
    {
        return await _context.Merchants.ToListAsync();
    }

    public async Task<Merchant> CreateAsync(Merchant merchant)
    {
        _context.Merchants.Add(merchant);
        await _context.SaveChangesAsync();
        return merchant;
    }

    public async Task<Merchant> UpdateAsync(Merchant merchant)
    {
        _context.Entry(merchant).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return merchant;
    }

    public async Task<bool> DeleteAsync(Guid merchantId)
    {
        var merchant = await _context.Merchants.FindAsync(merchantId);
        if (merchant == null) return false;

        _context.Merchants.Remove(merchant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid merchantId)
    {
        return await _context.Merchants.AnyAsync(m => m.MerchantId == merchantId);
    }
}
