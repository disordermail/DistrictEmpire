using System.Threading;
using System.Threading.Tasks;
using DistrictEmpire.Domain;

namespace DistrictEmpire.Application;

public interface IWalletGateway
{
    Task<WalletLedgerEntry> AddLedgerEntryAsync(
        string playerId,
        Money amount,
        string reason,
        string idempotencyKey,
        CancellationToken cancellationToken);
}
