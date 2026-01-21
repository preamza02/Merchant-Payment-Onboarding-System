namespace MerchantPayment.Application.Interfaces;

public interface IVelocityFraudChecker
{
    bool IsAllowed(Guid merchantId);
    void RecordTransaction(Guid merchantId);
}
