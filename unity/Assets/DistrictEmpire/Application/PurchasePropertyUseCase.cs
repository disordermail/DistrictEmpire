using System;
using System.Threading;
using System.Threading.Tasks;
using DistrictEmpire.Domain;

namespace DistrictEmpire.Application
{
    public sealed class PurchasePropertyUseCase
    {
        private readonly IServerClock serverClock;
        private readonly IWalletGateway walletGateway;

        public PurchasePropertyUseCase(IServerClock serverClock, IWalletGateway walletGateway)
        {
            this.serverClock = serverClock;
            this.walletGateway = walletGateway;
        }

        public async Task<WalletLedgerEntry> ExecuteAsync(string playerId, Money purchasePrice, string idempotencyKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ArgumentException("Idempotency key is required.", nameof(idempotencyKey));

            await serverClock.GetNowAsync(cancellationToken);

            return await walletGateway.AddLedgerEntryAsync(playerId, purchasePrice, "property_purchase", idempotencyKey, cancellationToken);
        }
    }
}
