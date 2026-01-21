using System.Collections.Concurrent;
using MerchantPayment.Application.Interfaces;

namespace MerchantPayment.Application.Services;

public class VelocityFraudChecker : IVelocityFraudChecker
{
    private readonly int _maxTransactions;
    private readonly TimeSpan _timeWindow;
    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<DateTime>> _merchantTransactions;

    public VelocityFraudChecker(int maxTransactions = 10, int timeWindowSeconds = 60)
    {
        _maxTransactions = maxTransactions;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
        _merchantTransactions = new ConcurrentDictionary<Guid, ConcurrentQueue<DateTime>>();
    }

    public bool IsAllowed(Guid merchantId)
    {
        var queue = _merchantTransactions.GetOrAdd(merchantId, _ => new ConcurrentQueue<DateTime>());
        PurgeExpiredEntries(queue);
        return queue.Count < _maxTransactions;
    }

    public void RecordTransaction(Guid merchantId)
    {
        var queue = _merchantTransactions.GetOrAdd(merchantId, _ => new ConcurrentQueue<DateTime>());
        queue.Enqueue(DateTime.UtcNow);
        PurgeExpiredEntries(queue);
    }

    private void PurgeExpiredEntries(ConcurrentQueue<DateTime> queue)
    {
        var cutoff = DateTime.UtcNow - _timeWindow;
        while (queue.TryPeek(out var oldest) && oldest < cutoff)
        {
            queue.TryDequeue(out _);
        }
    }
}
