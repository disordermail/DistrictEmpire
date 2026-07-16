using System;
using System.Collections.Generic;
using System.Linq;
using DistrictEmpire.Domain;

namespace DistrictEmpire.Application
{
    public sealed class GameService
    {
        private readonly IGameRepository repository;
        private readonly GameClock clock;
        public GameState State { get; private set; }

        public GameService(IGameRepository repository, GameClock clock)
        {
            this.repository = repository;
            this.clock = clock;
            State = repository.Load() ?? CreateNewGame();
            Tick();
        }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            clock.Advance(State, now);
            foreach (var property in State.Properties.Where(p => p.IsOwned))
            {
                if (property.Stage == PropertyStage.Notary && now.Ticks >= property.NotaryCompleteAtUtcTicks)
                    property.Stage = PropertyStage.ChoosingUse;
                if (property.Stage == PropertyStage.Listing && now.Ticks >= property.ListingAvailableAtUtcTicks && property.Applicants.Count == 0)
                {
                    property.Stage = PropertyStage.Applications;
                    property.Applicants = CreateApplicants(property.Use);
                }
            }
            repository.Save(State);
        }

        public bool CollectRent()
        {
            if (State.RentReady <= 0) return false;
            State.Cash += State.RentReady;
            State.RentReady = 0;
            repository.Save(State);
            return true;
        }

        public bool Repair(string propertyId)
        {
            var property = Find(propertyId);
            const int cost = 450;
            if (property == null || State.Cash < cost || property.Condition >= 100) return false;
            State.Cash -= cost;
            property.Condition = Math.Min(100, property.Condition + 22);
            repository.Save(State);
            return true;
        }

        public bool Buy(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || property.IsOwned || State.Cash < property.Price) return false;
            State.Cash -= property.Price;
            property.IsOwned = true;
            property.Stage = PropertyStage.Notary;
            property.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(12).Ticks;
            repository.Save(State);
            return true;
        }

        public void ChooseUse(string propertyId, PropertyUse use)
        {
            var property = Find(propertyId);
            if (property == null || property.Stage != PropertyStage.ChoosingUse) return;
            property.Use = use;
            property.Stage = PropertyStage.Available;
            repository.Save(State);
        }

        public void PublishListing(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || property.Stage != PropertyStage.Available || property.Use == PropertyUse.None) return;
            property.Stage = PropertyStage.Listing;
            property.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(8).Ticks;
            repository.Save(State);
        }

        public void SelectApplicant(string propertyId, string applicantId)
        {
            var property = Find(propertyId);
            var applicant = property?.Applicants.FirstOrDefault(a => a.Id == applicantId);
            if (property == null || applicant == null || property.Stage != PropertyStage.Applications) return;
            property.TenantName = applicant.Name;
            property.TenantDailyRent = applicant.DailyRent;
            property.Stage = PropertyStage.Occupied;
            property.Applicants.Clear();
            repository.Save(State);
        }

        public string Countdown(Property property) => clock.Countdown(property.Stage == PropertyStage.Notary ? property.NotaryCompleteAtUtcTicks : property.ListingAvailableAtUtcTicks, DateTime.UtcNow);
        private Property Find(string id) => State.Properties.FirstOrDefault(p => p.Id == id);

        private static GameState CreateNewGame()
        {
            var state = new GameState { LastClockUtcTicks = DateTime.UtcNow.Ticks, RentReady = 620 };
            state.Properties.Add(new Property { Id = "old-town", Name = "Old Town Apartment", District = "Warsaw", Icon = "Home", Price = 18000, BaseDailyRent = 620, Condition = 78, IsOwned = true, Stage = PropertyStage.Occupied, Use = PropertyUse.Residential, TenantName = "Maria Kowalska", TenantDailyRent = 620 });
            state.Properties.Add(new Property { Id = "riverside", Name = "Riverside Studio", District = "Praga", Icon = "Building", Price = 22000, BaseDailyRent = 760, Condition = 68, Stage = PropertyStage.Available });
            return state;
        }

        private static List<Applicant> CreateApplicants(PropertyUse use) => use == PropertyUse.Business
            ? new List<Applicant> { new() { Id = "pharmacy", Name = "Green Cross Pharmacy", Role = "Healthcare", Story = "A reliable neighborhood pharmacy with a 24-month lease.", DailyRent = 1040, IsBusiness = true }, new() { Id = "grocery", Name = "Corner Grocery", Role = "Retail", Story = "A local food store seeking a practical long-term base.", DailyRent = 900, IsBusiness = true } }
            : new List<Applicant> { new() { Id = "adam", Name = "Adam Nowak", Role = "Software engineer", Story = "Can move in tomorrow and prefers a one-year lease.", DailyRent = 760 }, new() { Id = "marta", Name = "Marta Zielinska", Role = "Teacher", Story = "Moving closer to school with her daughter.", DailyRent = 710 } };
    }
}
