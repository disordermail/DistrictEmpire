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
            Require(service.Repair(starter.Id), "Maintenance action failed.");

            var studio = RequireProperty(service, "riverside");
            Require(service.Buy(studio.Id), "Purchase action failed.");
            Require(studio.Stage == PropertyStage.Notary, "Purchase did not start notary transfer.");
            studio.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            Require(studio.Stage == PropertyStage.ChoosingUse, "Notary timer did not complete.");
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

            Debug.Log("District Empire core playtest passed: rent, repair, buy, notary, use choice, listing, applications and tenant selection.");
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
