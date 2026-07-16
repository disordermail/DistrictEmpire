using System;
using DistrictEmpire.Application;
using DistrictEmpire.Domain;
using UnityEditor;
using UnityEngine;

namespace DistrictEmpire.Editor
{
    public static class DistrictEmpirePlaytestRunner
    {
        [MenuItem("District Empire/Run Core Playtest")]
        public static void RunCorePlaytest()
        {
            var service = new GameService(new MemoryRepository(), new GameClock());
            var starter = RequireProperty(service, "old-town");
            Require(starter.Stage == PropertyStage.Occupied, "Starter property is not occupied.");
            Require(!string.IsNullOrEmpty(starter.TenantStory), "Starter tenant story is missing.");
            Require(service.CollectRent(), "Rent collection failed.");
            Require(service.State.RentReady == 0, "Collected rent remained in the wallet.");
            Require(service.ClaimDailyReward(), "Daily reward claim failed.");
            Require(!service.ClaimDailyReward(), "Daily reward can be claimed twice.");
            Require(service.BuyInfluence(), "Influence purchase failed.");
            Require(service.Repair(starter.Id), "Maintenance action failed.");
            Require(service.StartContractCancellation(starter.Id), "Contract cancellation failed.");
            starter.ContractEndAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            Require(starter.Stage == PropertyStage.Available && string.IsNullOrEmpty(starter.TenantName), "Tenant did not leave after cancellation.");
            Require(service.ListForSale(starter.Id), "Property sale listing failed.");
            var salePrice = starter.SalePrice;
            Require(service.AcceptSaleOffer(starter.Id), "Sale offer acceptance failed.");
            Require(!starter.IsOwned && service.State.Cash >= salePrice, "Property sale did not complete.");

            var cityEvent = service.State.Events[0];
            Require(service.ClaimEvent(cityEvent.Id), "City event claim failed.");
            Require(!service.ClaimEvent(cityEvent.Id), "City event can be claimed twice.");

            var studio = RequireProperty(service, "riverside");
            Require(service.Buy(studio.Id), "Purchase action failed.");
            Require(studio.Stage == PropertyStage.Notary, "Purchase did not start notary transfer.");
            Require(service.SpeedUpNotary(studio.Id), "Notary speed-up action failed.");
            Require(studio.Stage == PropertyStage.ChoosingUse, "Notary speed-up did not complete the transfer.");
            service.ChooseUse(studio.Id, PropertyUse.Business);
            Require(studio.Stage == PropertyStage.Available, "Business use choice failed.");
            service.PublishListing(studio.Id);
            Require(studio.Stage == PropertyStage.Listing, "Listing action failed.");
            studio.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            Require(studio.Stage == PropertyStage.Applications && studio.Applicants.Count > 0, "Applicants did not arrive.");
            service.SelectApplicant(studio.Id, studio.Applicants[0].Id);
            Require(studio.Stage == PropertyStage.Occupied, "Tenant selection failed.");
            Require(!string.IsNullOrEmpty(studio.TenantStory), "Selected tenant story is missing.");

            service.ResetProgress();
            Require(service.State.Cash == 32000, "Profile reset did not restore starter cash.");
            Require(service.State.RentReady == 620, "Profile reset did not restore starter rent.");
            Require(!service.State.DailyRewardClaimed, "Profile reset did not restore daily reward.");

            Debug.Log("District Empire core playtest passed: rent, shop, repair, contract cancellation, sale, event, buy, notary, use choice, listing, tenant selection and reset.");
        }

        [MenuItem("District Empire/Simulate 30 Days")]
        public static void SimulateThirtyDays()
        {
            var service = new GameService(new MemoryRepository(), new GameClock());
            var claimedRewards = 0;
            var claimedEvents = 0;
            var repairs = 0;
            for (var day = 1; day <= 30; day++)
            {
                AdvanceOneDay(service);
                if (!service.State.DailyRewardClaimed && service.ClaimDailyReward()) claimedRewards++;
                service.CollectRent();
                foreach (var property in service.State.Properties.FindAll(property => property.IsOwned && property.Condition < 90))
                    if (service.Repair(property.Id)) repairs++;

                foreach (var property in service.State.Properties.FindAll(property => !property.IsOwned && property.Price <= service.State.Cash))
                {
                    service.Buy(property.Id);
                    service.SpeedUpNotary(property.Id);
                    service.ChooseUse(property.Id, day % 2 == 0 ? PropertyUse.Business : PropertyUse.Residential);
                    service.PublishListing(property.Id);
                    property.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
                    service.Tick();
                    service.SelectApplicant(property.Id, property.Applicants[0].Id);
                }

                foreach (var cityEvent in service.State.Events.FindAll(cityEvent => !cityEvent.Claimed))
                    if (service.ClaimEvent(cityEvent.Id)) claimedEvents++;
                var owned = service.State.Properties.FindAll(property => property.IsOwned);
                Debug.Log("Day " + day + ": cash=" + service.State.Cash + ", owned=" + owned.Count + ", rentReady=" + service.State.RentReady + ", events=" + service.State.Events.Count);
            }

            var properties = service.State.Properties;
            Require(properties.Exists(property => property.IsOwned && property.Use == PropertyUse.Residential), "30-day simulation never retained a residential property.");
            Require(properties.Exists(property => property.IsOwned && property.Use == PropertyUse.Business), "30-day simulation never created a business property.");
            Require(service.State.Cash > 0, "30-day simulation exhausted all cash.");
            Require(claimedRewards == 30, "Daily reward did not refresh for all 30 simulated days.");
            Require(claimedEvents == 60, "City events did not refresh for all 30 simulated days.");
            Require(repairs >= 4, "Property wear did not create enough maintenance decisions.");
            Debug.Log("District Empire 30-day simulation passed.");
        }

        private static void AdvanceOneDay(GameService service)
        {
            service.State.LastClockUtcTicks = DateTime.UtcNow.AddSeconds(-1441).Ticks;
            service.Tick();
        }

        private static Property RequireProperty(GameService service, string id)
        {
            var property = service.State.Properties.Find(candidate => candidate.Id == id);
            if (property == null) throw new InvalidOperationException("Missing property: " + id);
            return property;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class MemoryRepository : IGameRepository
        {
            public GameState State;
            public GameState Load() => State;
            public void Save(GameState state) => State = state;
        }
    }
}
