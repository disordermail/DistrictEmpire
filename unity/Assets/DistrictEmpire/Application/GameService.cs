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
            EnsureMarketContent();
            EnsureLivingWorldContent();
            Tick();
        }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            var dayBefore = State.Day;
            clock.Advance(State, now);
            if (State.Day > dayBefore)
            {
                var elapsedDays = State.Day - dayBefore;
                State.DailyRewardClaimed = false;
                RefreshDailyEvents();
                foreach (var property in State.Properties.Where(property => property.IsOwned && property.Stage == PropertyStage.Occupied))
                    property.Condition = Math.Max(55, property.Condition - elapsedDays * 2);
                RefreshNpcActivities();
                UpdateGoals();
            }
            foreach (var property in State.Properties.Where(p => p.IsOwned))
            {
                if (property.Stage == PropertyStage.Notary && now.Ticks >= property.NotaryCompleteAtUtcTicks)
                    property.Stage = PropertyStage.ChoosingUse;
                if (property.Stage == PropertyStage.Listing && now.Ticks >= property.ListingAvailableAtUtcTicks && property.Applicants.Count == 0)
                {
                    property.Stage = PropertyStage.Applications;
                    property.Applicants = CreateApplicants(property.Use);
                }
                if (property.Stage == PropertyStage.CancellingContract && now.Ticks >= property.ContractEndAtUtcTicks)
                    ClearTenant(property);
            }
            repository.Save(State);
        }

        public bool CollectRent()
        {
            if (State.RentReady <= 0) return false;
            State.Cash += State.RentReady;
            State.RentReady = 0;
            State.Xp += 25;
            State.Influence += 1;
            UpdateCompanyLevel();
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
            State.Xp += 15;
            UpdateCompanyLevel();
            repository.Save(State);
            return true;
        }

        public bool Renovate(string propertyId)
        {
            var property = Find(propertyId);
            var cost = 900 + property?.Tier * 300 ?? 0;
            if (property == null || !property.IsOwned || property.Renovated || State.Cash < cost) return false;
            State.Cash -= cost;
            property.Renovated = true;
            property.Condition = 100;
            property.BaseDailyRent += 90 * property.Tier;
            property.TenantDailyRent += property.Stage == PropertyStage.Occupied ? 90 * property.Tier : 0;
            State.Xp += 25;
            UpdateCompanyLevel();
            repository.Save(State);
            return true;
        }

        public bool UpgradeProperty(string propertyId)
        {
            var property = Find(propertyId);
            var cost = property == null ? 0 : 1100 * property.Level * property.Tier;
            if (property == null || !property.IsOwned || State.Cash < cost || property.Level >= 3) return false;
            State.Cash -= cost;
            property.Level++;
            property.BaseDailyRent += 70 * property.Tier;
            if (property.Stage == PropertyStage.Occupied) property.TenantDailyRent += 70 * property.Tier;
            State.Xp += 20;
            UpdateCompanyLevel();
            UpdateGoals();
            repository.Save(State);
            return true;
        }

        public bool PromoteProperty(string propertyId)
        {
            const int cost = 3;
            var property = Find(propertyId);
            if (property == null || !property.IsOwned || State.Influence < cost || property.Popularity >= 3) return false;
            State.Influence -= cost;
            property.Popularity++;
            property.BaseDailyRent += 35;
            if (property.Stage == PropertyStage.Occupied) property.TenantDailyRent += 35;
            repository.Save(State);
            return true;
        }

        public bool Buy(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || property.IsOwned || State.Cash < property.Price) return false;
            State.Cash -= property.Price;
            property.IsOwned = true;
            property.BuildingOwnedUnits = Math.Max(1, property.BuildingOwnedUnits);
            property.Stage = PropertyStage.Notary;
            property.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(12).Ticks;
            UpdateGoals();
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

        public bool SpeedUpNotary(string propertyId)
        {
            const int influenceCost = 5;
            var property = Find(propertyId);
            if (property == null || property.Stage != PropertyStage.Notary || State.Influence < influenceCost) return false;
            State.Influence -= influenceCost;
            property.NotaryCompleteAtUtcTicks = DateTime.UtcNow.Ticks;
            Tick();
            return true;
        }

        public bool ClaimDailyReward()
        {
            if (State.DailyRewardClaimed) return false;
            State.DailyRewardClaimed = true;
            State.Cash += 750;
            State.Xp += 15;
            UpdateCompanyLevel();
            repository.Save(State);
            return true;
        }

        public bool BuyInfluence()
        {
            const int cashCost = 600;
            if (State.Cash < cashCost) return false;
            State.Cash -= cashCost;
            State.Influence += 5;
            repository.Save(State);
            return true;
        }

        public void ResetProgress()
        {
            State = CreateNewGame();
            EnsureLivingWorldContent();
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
            property.TenantRole = applicant.Role;
            property.TenantStory = applicant.Story;
            property.Relationship = 54;
            property.TenantDailyRent = applicant.DailyRent;
            property.Stage = PropertyStage.Occupied;
            property.Applicants.Clear();
            State.Xp += 30;
            UpdateCompanyLevel();
            repository.Save(State);
        }

        public bool NegotiateApplicant(string propertyId, string applicantId)
        {
            var property = Find(propertyId);
            var applicant = property?.Applicants.FirstOrDefault(candidate => candidate.Id == applicantId);
            if (property == null || applicant == null || applicant.Negotiated || property.Stage != PropertyStage.Applications) return false;
            applicant.Negotiated = true;
            applicant.DailyRent += applicant.IsBusiness ? 120 : 60;
            repository.Save(State);
            return true;
        }

        public bool StartContractCancellation(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || property.Stage != PropertyStage.Occupied) return false;
            property.Stage = PropertyStage.CancellingContract;
            property.ContractEndAtUtcTicks = DateTime.UtcNow.AddSeconds(12).Ticks;
            repository.Save(State);
            return true;
        }

        public bool ListForSale(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || !property.IsOwned || property.Stage == PropertyStage.Occupied || property.Stage == PropertyStage.CancellingContract || property.Stage == PropertyStage.Notary) return false;
            property.Stage = PropertyStage.ForSale;
            property.SalePrice = property.Price + property.Price / 8;
            repository.Save(State);
            return true;
        }

        public bool AcceptSaleOffer(string propertyId)
        {
            var property = Find(propertyId);
            if (property == null || property.Stage != PropertyStage.ForSale) return false;
            State.Cash += property.SalePrice;
            State.Xp += 40;
            property.IsOwned = false;
            property.Stage = PropertyStage.Available;
            property.Use = PropertyUse.None;
            property.SalePrice = 0;
            property.BuildingOwnedUnits = Math.Max(0, property.BuildingOwnedUnits - 1);
            UpdateCompanyLevel();
            UpdateGoals();
            repository.Save(State);
            return true;
        }

        public bool ClaimEvent(string eventId)
        {
            var cityEvent = State.Events.FirstOrDefault(item => item.Id == eventId);
            if (cityEvent == null || cityEvent.Claimed) return false;
            cityEvent.Claimed = true;
            State.Influence += 3;
            State.Xp += 10;
            UpdateCompanyLevel();
            repository.Save(State);
            return true;
        }

        public bool ClaimGoal(string goalId)
        {
            var goal = State.Goals.FirstOrDefault(candidate => candidate.Id == goalId);
            if (goal == null || goal.Claimed || goal.Progress < goal.Target) return false;
            goal.Claimed = true;
            State.Cash += 500;
            State.Influence += 2;
            State.Xp += 20;
            UpdateCompanyLevel();
            repository.Save(State);
            return true;
        }

        public string Countdown(Property property) => clock.Countdown(property.Stage == PropertyStage.Notary ? property.NotaryCompleteAtUtcTicks : property.Stage == PropertyStage.CancellingContract ? property.ContractEndAtUtcTicks : property.ListingAvailableAtUtcTicks, DateTime.UtcNow);
        private Property Find(string id) => State.Properties.FirstOrDefault(p => p.Id == id);

        private static void ClearTenant(Property property)
        {
            property.Stage = PropertyStage.Available;
            property.TenantName = null;
            property.TenantRole = null;
            property.TenantStory = null;
            property.Relationship = 0;
            property.TenantDailyRent = 0;
            property.ContractEndAtUtcTicks = 0;
        }

        private static GameState CreateNewGame()
        {
            var state = new GameState { LastClockUtcTicks = DateTime.UtcNow.Ticks, RentReady = 620 };
            state.Properties.Add(new Property { Id = "old-town", Name = "Mokotow Starter", District = "Mokotow", Icon = "HOME", Price = 18000, BaseDailyRent = 620, Tier = 1, Category = "Apartments", MapX = 18, MapY = 58, Condition = 78, IsOwned = true, Stage = PropertyStage.Occupied, Use = PropertyUse.Residential, TenantName = "Maria Kowalska", TenantRole = "Teacher · single mother", TenantStory = "Maria teaches nearby and is building a new life in Mokotow.", Relationship = 62, TenantDailyRent = 620, BuildingName = "Mokotow Gardens", BuildingOwnedUnits = 3, BuildingTotalUnits = 10 });
            AddMarketProperties(state);
            RefreshDailyEvents(state);
            return state;
        }

        private void EnsureMarketContent()
        {
            var starter = State.Properties.FirstOrDefault(p => p.Id == "old-town");
            if (starter != null)
            {
                starter.Name = "Mokotow Starter";
                starter.District = "Mokotow";
                starter.Icon = "HOME";
                starter.Tier = 1;
                starter.Category = "Apartments";
                starter.MapX = 18;
                starter.MapY = 58;
            }
            if (!State.Properties.Any(p => p.Id == "wola-corner")) AddMarketProperties(State);
            repository.Save(State);
        }

        private void EnsureLivingWorldContent()
        {
            var starter = Find("old-town");
            if (starter != null)
            {
                if (string.IsNullOrEmpty(starter.TenantRole)) starter.TenantRole = "Teacher · single mother";
                if (string.IsNullOrEmpty(starter.TenantStory)) starter.TenantStory = "Maria teaches nearby and is building a new life in Mokotow.";
                if (starter.Relationship <= 0) starter.Relationship = 62;
                if (string.IsNullOrEmpty(starter.BuildingName)) starter.BuildingName = "Mokotow Gardens";
                if (starter.BuildingTotalUnits <= 0) starter.BuildingTotalUnits = 10;
                if (starter.BuildingOwnedUnits <= 0) starter.BuildingOwnedUnits = 3;
            }
            foreach (var property in State.Properties.Where(p => p.BuildingTotalUnits <= 0))
            {
                property.BuildingName = property.Name + " Residences";
                property.BuildingTotalUnits = property.Tier == 3 ? 18 : property.Tier == 2 ? 12 : 8;
                property.BuildingOwnedUnits = property.IsOwned ? 1 : 0;
            }
            if (State.Events == null) State.Events = new List<CityEvent>();
            if (State.NpcActivities == null) State.NpcActivities = new List<NpcActivity>();
            if (State.Goals == null) State.Goals = new List<CompanyGoal>();
            if (State.EventsDay != State.Day || State.Events.Count == 0) RefreshDailyEvents();
            if (State.NpcActivities.Count == 0) RefreshNpcActivities();
            EnsureGoals();
            UpdateCompanyLevel();
            repository.Save(State);
        }

        private void UpdateCompanyLevel()
        {
            State.CompanyLevel = Math.Max(1, 1 + State.Xp / 100);
        }

        private void EnsureGoals()
        {
            if (State.Goals.Count > 0) return;
            State.Goals.Add(new CompanyGoal { Id = "first-upgrade", Title = "Raise the standard", Detail = "Upgrade any owned property.", Target = 1 });
            State.Goals.Add(new CompanyGoal { Id = "two-assets", Title = "Build your district", Detail = "Own two properties.", Target = 2 });
            UpdateGoals();
        }

        private void UpdateGoals()
        {
            foreach (var goal in State.Goals)
            {
                goal.Progress = goal.Id == "first-upgrade"
                    ? State.Properties.Count(property => property.IsOwned && property.Level > 1)
                    : State.Properties.Count(property => property.IsOwned);
            }
        }

        private void RefreshNpcActivities()
        {
            State.NpcActivities.Clear();
            var target = State.Properties.FirstOrDefault(property => !property.IsOwned) ?? State.Properties.First();
            var first = State.Day % 2 == 0 ? "Nova Estates" : "Anna Kowalska";
            State.NpcActivities.Add(new NpcActivity { Investor = first, Title = State.Day % 2 == 0 ? "opened a pharmacy" : "bought a unit nearby", Detail = target.BuildingName + " · market activity on day " + State.Day });
            State.NpcActivities.Add(new NpcActivity { Investor = "Riverside Investments", Title = "is watching an auction", Detail = target.Name + " · bid window closes tomorrow" });
        }

        private static void AddMarketProperties(GameState state)
        {
            state.Properties.Add(new Property { Id = "riverside", Name = "Riverside Studio", District = "Praga", Icon = "REN", Price = 22000, BaseDailyRent = 760, Tier = 1, Category = "Renovation", MapX = 68, MapY = 45, Condition = 68, Stage = PropertyStage.Available });
            state.Properties.Add(new Property { Id = "wola-corner", Name = "Wola Corner", District = "Wola", Icon = "MIX", Price = 36000, BaseDailyRent = 1180, Tier = 2, Category = "Mixed use", MapX = 38, MapY = 36, Condition = 86, Stage = PropertyStage.Available });
            state.Properties.Add(new Property { Id = "zoliborz-arcade", Name = "Zoliborz Arcade", District = "Zoliborz", Icon = "RET", Price = 45500, BaseDailyRent = 1460, Tier = 2, Category = "Retail", MapX = 28, MapY = 19, Condition = 91, Stage = PropertyStage.Available });
            state.Properties.Add(new Property { Id = "srodmiescie-house", Name = "Srodmiescie House", District = "Srodmiescie", Icon = "PRE", Price = 74000, BaseDailyRent = 2360, Tier = 3, Category = "Premium", MapX = 49, MapY = 61, Condition = 95, Stage = PropertyStage.Available });
        }

        private void RefreshDailyEvents()
        {
            RefreshDailyEvents(State);
        }

        private static void RefreshDailyEvents(GameState state)
        {
            state.Events.Clear();
            state.EventsDay = state.Day;
            var evenDay = state.Day % 2 == 0;
            state.Events.Add(new CityEvent { Id = "festival-" + state.Day, Title = evenDay ? "Night Market" : "Summer Festival", Detail = evenDay ? "Local shops are looking for more foot traffic tonight." : "Tourism demand is rising. Promote your next business listing.", Reward = "+3 influence · +10 XP" });
            state.Events.Add(new CityEvent { Id = "district-" + state.Day, Title = evenDay ? "Landlord Meeting" : "District Cleanup", Detail = evenDay ? "Meet local owners and learn about an upcoming auction." : "Support Mokotow residents and strengthen your company reputation.", Reward = "+3 influence · +10 XP" });
        }

        private static List<Applicant> CreateApplicants(PropertyUse use) => use == PropertyUse.Business
            ? new List<Applicant> { new() { Id = "pharmacy", Name = "Green Cross Pharmacy", Role = "Pharmacy", Story = "A reliable neighborhood pharmacy with a 24-month lease.", DailyRent = 1040, IsBusiness = true }, new() { Id = "grocery", Name = "Corner Grocery", Role = "Grocery shop", Story = "A local food store seeking a practical long-term base.", DailyRent = 900, IsBusiness = true }, new() { Id = "pierogi", Name = "Pierogi Bar", Role = "Restaurant", Story = "A family restaurant ready to open on Friday.", DailyRent = 1120, IsBusiness = true }, new() { Id = "beauty", Name = "Nova Beauty Studio", Role = "Beauty salon", Story = "A growing studio with loyal neighborhood customers.", DailyRent = 980, IsBusiness = true } }
            : new List<Applicant> { new() { Id = "adam", Name = "Adam Nowak", Role = "Software engineer", Story = "Can move in tomorrow and prefers a one-year lease.", DailyRent = 760 }, new() { Id = "marta", Name = "Marta Zielinska", Role = "Teacher", Story = "Moving closer to school with her daughter.", DailyRent = 710 } };
    }
}
