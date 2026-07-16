using System;

namespace DistrictEmpire.Domain
{
    public sealed class WalletLedgerEntry
    {
        public WalletLedgerEntry(string id, string playerId, Money amount, string reason, string idempotencyKey, DateTimeOffset createdAt)
        {
            Id = id; PlayerId = playerId; Amount = amount; Reason = reason; IdempotencyKey = idempotencyKey; CreatedAt = createdAt;
        }
        public string Id { get; private set; }
        public string PlayerId { get; private set; }
        public Money Amount { get; private set; }
        public string Reason { get; private set; }
        public string IdempotencyKey { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
    }
}
