using FluentAssertions;
using MerchantPayment.Application.Services;
using Xunit;

namespace MerchantPayment.Tests.Services;

public class VelocityFraudCheckerTests
{
    [Fact]
    public void IsAllowed_WhenNoTransactions_ShouldReturnTrue()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 10, timeWindowSeconds: 60);

        var result = velocityFraudChecker.IsAllowed(merchantId);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAllowed_WhenUnderLimit_ShouldReturnTrue()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 10, timeWindowSeconds: 60);

        for (int i = 0; i < 5; i++)
        {
            velocityFraudChecker.RecordTransaction(merchantId);
        }

        var result = velocityFraudChecker.IsAllowed(merchantId);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAllowed_WhenAtLimit_ShouldReturnFalse()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 10, timeWindowSeconds: 60);

        for (int i = 0; i < 10; i++)
        {
            velocityFraudChecker.RecordTransaction(merchantId);
        }

        var result = velocityFraudChecker.IsAllowed(merchantId);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsAllowed_WhenOverLimit_ShouldReturnFalse()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 10, timeWindowSeconds: 60);

        for (int i = 0; i < 15; i++)
        {
            velocityFraudChecker.RecordTransaction(merchantId);
        }

        var result = velocityFraudChecker.IsAllowed(merchantId);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsAllowed_WithCustomLimits_ShouldRespectLimits()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 3, timeWindowSeconds: 30);

        for (int i = 0; i < 3; i++)
        {
            velocityFraudChecker.RecordTransaction(merchantId);
        }

        var result = velocityFraudChecker.IsAllowed(merchantId);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsAllowed_DifferentMerchants_ShouldTrackSeparately()
    {
        var merchantId1 = Guid.NewGuid();
        var merchantId2 = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 5, timeWindowSeconds: 60);

        for (int i = 0; i < 5; i++)
        {
            velocityFraudChecker.RecordTransaction(merchantId1);
        }

        velocityFraudChecker.IsAllowed(merchantId1).Should().BeFalse();
        velocityFraudChecker.IsAllowed(merchantId2).Should().BeTrue();
    }

    [Fact]
    public void RecordTransaction_ShouldAddToCount()
    {
        var merchantId = Guid.NewGuid();
        var velocityFraudChecker = new VelocityFraudChecker(maxTransactions: 2, timeWindowSeconds: 60);

        velocityFraudChecker.IsAllowed(merchantId).Should().BeTrue();
        
        velocityFraudChecker.RecordTransaction(merchantId);
        velocityFraudChecker.IsAllowed(merchantId).Should().BeTrue();
        
        velocityFraudChecker.RecordTransaction(merchantId);
        velocityFraudChecker.IsAllowed(merchantId).Should().BeFalse();
    }
}
