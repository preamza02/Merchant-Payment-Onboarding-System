using FluentAssertions;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Application.Services;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Domain.Enums;
using Moq;
using Xunit;

namespace MerchantPayment.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IMerchantRepository> _merchantRepositoryMock;
    private readonly Mock<IVelocityFraudChecker> _velocityFraudCheckerMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _merchantRepositoryMock = new Mock<IMerchantRepository>();
        _velocityFraudCheckerMock = new Mock<IVelocityFraudChecker>();
        _paymentService = new PaymentService(
            _transactionRepositoryMock.Object,
            _merchantRepositoryMock.Object,
            _velocityFraudCheckerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTransactionExists_ShouldReturnPaymentResponse()
    {
        var transactionId = Guid.NewGuid();
        var transaction = new PaymentTransaction
        {
            TransactionId = transactionId,
            MerchantId = Guid.NewGuid(),
            Amount = 100.00m,
            Currency = "THB",
            Status = TransactionStatus.Success
        };
        _transactionRepositoryMock.Setup(x => x.GetByIdAsync(transactionId)).ReturnsAsync(transaction);

        var result = await _paymentService.GetByIdAsync(transactionId);

        result.Should().NotBeNull();
        result.TransactionId.Should().Be(transactionId);
        result.Amount.Should().Be(100.00m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTransactionNotExists_ShouldThrowKeyNotFoundException()
    {
        var transactionId = Guid.NewGuid();
        _transactionRepositoryMock.Setup(x => x.GetByIdAsync(transactionId)).ReturnsAsync((PaymentTransaction?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _paymentService.GetByIdAsync(transactionId));
    }

    [Fact]
    public async Task GetByMerchantIdAsync_ShouldReturnMerchantTransactions()
    {
        var merchantId = Guid.NewGuid();
        var transactions = new List<PaymentTransaction>
        {
            new() { TransactionId = Guid.NewGuid(), MerchantId = merchantId, Amount = 100.00m },
            new() { TransactionId = Guid.NewGuid(), MerchantId = merchantId, Amount = 200.00m }
        };
        _merchantRepositoryMock.Setup(x => x.ExistsAsync(merchantId)).ReturnsAsync(true);
        _transactionRepositoryMock.Setup(x => x.GetByMerchantIdAsync(merchantId)).ReturnsAsync(transactions);

        var result = await _paymentService.GetByMerchantIdAsync(merchantId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenMerchantNotActive_ShouldThrowInvalidOperationException()
    {
        var request = new CreatePaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Amount = 100.00m,
            Currency = "THB"
        };

        var merchant = new Merchant
        {
            MerchantId = request.MerchantId,
            Status = MerchantStatus.Pending
        };
        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(request.MerchantId)).ReturnsAsync(merchant);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _paymentService.CreatePaymentAsync(request));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenMerchantNotFound_ShouldThrowKeyNotFoundException()
    {
        var request = new CreatePaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Amount = 100.00m,
            Currency = "THB"
        };

        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(request.MerchantId)).ReturnsAsync((Merchant?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _paymentService.CreatePaymentAsync(request));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenFraudDetected_ShouldThrowInvalidOperationException()
    {
        var request = new CreatePaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Amount = 100.00m,
            Currency = "THB"
        };

        var merchant = new Merchant
        {
            MerchantId = request.MerchantId,
            Status = MerchantStatus.Active
        };
        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(request.MerchantId)).ReturnsAsync(merchant);
        _velocityFraudCheckerMock.Setup(x => x.IsAllowed(request.MerchantId))
            .Returns(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _paymentService.CreatePaymentAsync(request));
    }

    [Fact]
    public async Task CreatePaymentAsync_WhenValid_ShouldCreatePayment()
    {
        var request = new CreatePaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Amount = 100.00m,
            Currency = "THB"
        };

        var merchant = new Merchant
        {
            MerchantId = request.MerchantId,
            Status = MerchantStatus.Active
        };
        _merchantRepositoryMock.Setup(x => x.GetByIdAsync(request.MerchantId)).ReturnsAsync(merchant);
        _velocityFraudCheckerMock.Setup(x => x.IsAllowed(request.MerchantId))
            .Returns(true);
        _transactionRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<PaymentTransaction>()))
            .ReturnsAsync((PaymentTransaction t) => t);

        var result = await _paymentService.CreatePaymentAsync(request);

        result.Should().NotBeNull();
        result.Amount.Should().Be(100.00m);
        result.Currency.Should().Be("THB");
        _transactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<PaymentTransaction>()), Times.Once);
    }
}
