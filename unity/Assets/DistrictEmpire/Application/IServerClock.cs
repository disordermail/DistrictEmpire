using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistrictEmpire.Application;

public interface IServerClock
{
    Task<DateTimeOffset> GetNowAsync(CancellationToken cancellationToken);
}
