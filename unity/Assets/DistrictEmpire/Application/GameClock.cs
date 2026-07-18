using System;
using DistrictEmpire.Domain;

namespace DistrictEmpire.Application
{
    public sealed class GameClock
    {
        // One real second equals one simulated minute. Progress survives app restarts.
        public void Advance(GameState state, DateTime utcNow)
        {
            if (state.LastClockUtcTicks == 0) { state.LastClockUtcTicks = utcNow.Ticks; return; }
            var elapsed = utcNow - new DateTime(state.LastClockUtcTicks, DateTimeKind.Utc);
            var gameMinutes = Math.Max(0, (int)elapsed.TotalSeconds);
            if (gameMinutes >= 1440)
            {
                var days = gameMinutes / 1440;
                state.Day += days;
                foreach (var property in state.Properties)
                    if (property.IsOwned && property.Stage == PropertyStage.Occupied)
                        state.RentReady += property.TenantDailyRent * (100 + state.LandlordSkill) / 100 * days;
            }
            state.LastClockUtcTicks = utcNow.Ticks;
        }

        public string Countdown(long targetTicks, DateTime utcNow)
        {
            var remaining = new DateTime(targetTicks, DateTimeKind.Utc) - utcNow;
            if (remaining <= TimeSpan.Zero) return "Ready now";
            return $"{Math.Max(1, (int)Math.Ceiling(remaining.TotalSeconds))}m remaining";
        }
    }
}
