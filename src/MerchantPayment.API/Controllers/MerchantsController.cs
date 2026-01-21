using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;

namespace MerchantPayment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MerchantsController : ControllerBase
{
    private readonly IMerchantService _merchantService;

    public MerchantsController(IMerchantService merchantService)
    {
        _merchantService = merchantService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<MerchantResponse>>>> GetAll()
    {
        var merchants = await _merchantService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<MerchantResponse>>.SuccessResponse(merchants));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MerchantResponse>>> GetById(Guid id)
    {
        var merchant = await _merchantService.GetByIdAsync(id);
        return Ok(ApiResponse<MerchantResponse>.SuccessResponse(merchant));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MerchantResponse>>> Create([FromBody] CreateMerchantRequest request)
    {
        var merchant = await _merchantService.CreateAsync(request);
        return CreatedAtAction(
            nameof(GetById),
            new { id = merchant.MerchantId },
            ApiResponse<MerchantResponse>.SuccessResponse(merchant, "Merchant created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MerchantResponse>>> Update(Guid id, [FromBody] UpdateMerchantRequest request)
    {
        var merchant = await _merchantService.UpdateAsync(id, request);
        return Ok(ApiResponse<MerchantResponse>.SuccessResponse(merchant, "Merchant updated successfully"));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<MerchantResponse>>> UpdateStatus(Guid id, [FromBody] UpdateMerchantStatusRequest request)
    {
        var merchant = await _merchantService.UpdateStatusAsync(id, request);
        return Ok(ApiResponse<MerchantResponse>.SuccessResponse(merchant, "Merchant status updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await _merchantService.DeleteAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Merchant deleted successfully"));
    }
}
