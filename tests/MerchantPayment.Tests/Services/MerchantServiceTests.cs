using FluentAssertions;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Application.Services;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Domain.Enums;
using Moq;
using Xunit;

namespace MerchantPayment.Tests.Services;

public class MerchantServiceTests
{
    private readonly Mock<IMerchantRepository> _merchantRepositoryMock;
    private readonly MerchantService _merchantService;

    public MerchantServiceTests()
    {
        _merchantRepositoryMock = new Mock<IMerchantRepository>();
        _merchantService = new MerchantService(_merchantRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMerchants()
    {
        var merchants = new List<Merchant>
        {
            new() { MerchantId = Guid.NewGuid(), BusinessName = "Test Business 1", Email = "test1@example.com" },
            new() { MerchantId = Guid.NewGuid(), BusinessName = "Test Business 2", Email = "test2@example.com" }
        };
        _merchantRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(merchants);

        var result = await _merchantService.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _merchantRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMerchantExists_ShouldReturnMerchant()
    {
        var merchantId = Guid.NewGuid();
        var merchant = new Merchant
        {
            MerchantId = merchantId,
            BusinessName = "Test Business",
            Email = "test@example.com",
            Status = MerchantStatus.Active
        };
        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(merchantId)).ReturnsAsync(merchant);

        var result = await _merchantService.GetByIdAsync(merchantId);

        result.Should().NotBeNull();
        result.MerchantId.Should().Be(merchantId);
        result.BusinessName.Should().Be("Test Business");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMerchantNotExists_ShouldThrowKeyNotFoundException()
    {
        var merchantId = Guid.NewGuid();
        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(merchantId)).ReturnsAsync((Merchant?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _merchantService.GetByIdAsync(merchantId));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnMerchant()
    {
        var request = new CreateMerchantRequest
        {
            BusinessName = "New Business",
            Email = "newbusiness@example.com"
        };

        _merchantRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((Merchant?)null);
        _merchantRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Merchant>()))
            .ReturnsAsync((Merchant m) => m);

        var result = await _merchantService.CreateAsync(request);

        result.Should().NotBeNull();
        result.BusinessName.Should().Be(request.BusinessName);
        result.Email.Should().Be(request.Email);
        result.Status.Should().Be(MerchantStatus.Pending.ToString());
        _merchantRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Merchant>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailExists_ShouldThrowInvalidOperationException()
    {
        var request = new CreateMerchantRequest
        {
            BusinessName = "New Business",
            Email = "existing@example.com"
        };

        var existingMerchant = new Merchant { MerchantId = Guid.NewGuid(), Email = request.Email };
        _merchantRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(existingMerchant);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _merchantService.CreateAsync(request));
    }

    [Fact]
    public async Task DeleteAsync_WhenMerchantExists_ShouldReturnTrue()
    {
        var merchantId = Guid.NewGuid();
        _merchantRepositoryMock.Setup(x => x.ExistsAsync(merchantId)).ReturnsAsync(true);
        _merchantRepositoryMock.Setup(x => x.DeleteAsync(merchantId)).ReturnsAsync(true);

        var result = await _merchantService.DeleteAsync(merchantId);

        result.Should().BeTrue();
        _merchantRepositoryMock.Verify(x => x.DeleteAsync(merchantId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMerchantNotExists_ShouldThrowKeyNotFoundException()
    {
        var merchantId = Guid.NewGuid();
        _merchantRepositoryMock.Setup(x => x.ExistsAsync(merchantId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _merchantService.DeleteAsync(merchantId));
    }
}
