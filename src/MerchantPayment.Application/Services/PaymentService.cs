using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;
using MerchantPayment.Domain.Entities;
using MerchantPayment.Domain.Enums;

namespace MerchantPayment.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMerchantRepository _merchantRepository;
    private readonly IVelocityFraudChecker _velocityFraudChecker;

    public PaymentService(
        ITransactionRepository transactionRepository,
        IMerchantRepository merchantRepository,
        IVelocityFraudChecker velocityFraudChecker)
    {
        _transactionRepository = transactionRepository;
        _merchantRepository = merchantRepository;
        _velocityFraudChecker = velocityFraudChecker;
    }

    public async Task<PaymentResponse> GetByIdAsync(Guid transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId)
            ?? throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");

        return MapToResponse(transaction);
    }

    public async Task<IEnumerable<PaymentResponse>> GetByMerchantIdAsync(Guid merchantId)
    {
        if (!await _merchantRepository.ExistsAsync(merchantId))
        {
            throw new KeyNotFoundException($"Merchant with ID {merchantId} not found");
        }

        var transactions = await _transactionRepository.GetByMerchantIdAsync(merchantId);
        return transactions.Select(MapToResponse);
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId)
            ?? throw new KeyNotFoundException($"Merchant with ID {request.MerchantId} not found");

        if (merchant.Status != MerchantStatus.Active)
        {
            throw new InvalidOperationException($"Merchant is not active. Current status: {merchant.Status}");
        }

        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var existingTransaction = await _transactionRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey);
            if (existingTransaction != null)
            {
                return MapToResponse(existingTransaction);
            }
        }

        if (!_velocityFraudChecker.IsAllowed(request.MerchantId))
        {
            throw new InvalidOperationException("Transaction rate limit exceeded. Please try again later.");
        }

        var transaction = new PaymentTransaction
        {
            TransactionId = Guid.NewGuid(),
            MerchantId = request.MerchantId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = TransactionStatus.Pending,
            IdempotencyKey = request.IdempotencyKey,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTransaction = await _transactionRepository.CreateAsync(transaction);
        
        _velocityFraudChecker.RecordTransaction(request.MerchantId);
        
        return MapToResponse(createdTransaction);
    }

    public async Task<PaymentResponse> ProcessCallbackAsync(PaymentCallbackRequest request)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId)
            ?? throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found");

        if (transaction.Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException($"Transaction is not pending. Current status: {transaction.Status}");
        }

        var previousStatus = transaction.Status.ToString();
        transaction.Status = Enum.Parse<TransactionStatus>(request.Status);
        transaction.ExternalReferenceId = request.ExternalReferenceId;
        transaction.UpdatedAt = DateTime.UtcNow;

        var auditLog = new TransactionAuditLog
        {
            AuditLogId = Guid.NewGuid(),
            TransactionId = transaction.TransactionId,
            PreviousStatus = previousStatus,
            NewStatus = transaction.Status.ToString(),
            Message = request.Message ?? $"Status changed via callback",
            CreatedAt = DateTime.UtcNow
        };

        var updatedTransaction = await _transactionRepository.UpdateWithAuditAsync(transaction, auditLog);
        return MapToResponse(updatedTransaction);
    }

    private static PaymentResponse MapToResponse(PaymentTransaction transaction)
    {
        return new PaymentResponse
        {
            TransactionId = transaction.TransactionId,
            MerchantId = transaction.MerchantId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Status = transaction.Status.ToString(),
            ExternalReferenceId = transaction.ExternalReferenceId,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}
