using System;

namespace DistrictEmpire.Domain;

public sealed record WalletLedgerEntry(
    string Id,
    string PlayerId,
    Money Amount,
    string Reason,
    string IdempotencyKey,
    DateTimeOffset CreatedAt);
