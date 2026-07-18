using System;
using DistrictEmpire.Application;
using DistrictEmpire.Domain;
using NUnit.Framework;

namespace DistrictEmpire.Tests
{
    public sealed class GameServiceFlowTests
    {
        [Test]
        public void StarterProperty_ProvidesRentStoryAndMaintenanceTask()
        {
            var service = NewService();
            var starter = service.State.Properties.Find(property => property.Id == "old-town");

            Assert.NotNull(starter);
            Assert.AreEqual(PropertyStage.Occupied, starter.Stage);
            Assert.AreEqual("Maria Kowalska", starter.TenantName);
            Assert.Greater(starter.Relationship, 0);
            Assert.AreEqual(10, starter.BuildingTotalUnits);
            Assert.Greater(service.State.RentReady, 0);
        }

        [Test]
        public void CollectRent_UpdatesCashInfluenceAndProgression()
        {
            var service = NewService();
            var openingCash = service.State.Cash;
            var openingXp = service.State.Xp;

            Assert.IsTrue(service.CollectRent());

            Assert.AreEqual(openingCash + 620, service.State.Cash);
            Assert.AreEqual(0, service.State.RentReady);
            Assert.AreEqual(openingXp + 25, service.State.Xp);
            Assert.AreEqual(36, service.State.Influence);
            Assert.IsFalse(service.CollectRent());
        }

        [Test]
        public void Repair_ImprovesConditionAndAwardsXp()
        {
            var service = NewService();
            var starter = service.State.Properties.Find(property => property.Id == "old-town");
            var openingCash = service.State.Cash;

            Assert.IsTrue(service.Repair(starter.Id));

            Assert.AreEqual(100, starter.Condition);
            Assert.AreEqual(openingCash - 450, service.State.Cash);
            Assert.AreEqual(15, service.State.Xp);
        }

        [Test]
        public void BuyToTenantSelection_CompletesResidentialLifecycle()
        {
            var service = NewService();
            var studio = service.State.Properties.Find(property => property.Id == "riverside");

            Assert.IsTrue(service.Buy(studio.Id));
            Assert.AreEqual(PropertyStage.Notary, studio.Stage);

            studio.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            Assert.AreEqual(PropertyStage.ChoosingUse, studio.Stage);

            service.ChooseUse(studio.Id, PropertyUse.Residential);
            Assert.AreEqual(PropertyStage.Available, studio.Stage);
            service.PublishListing(studio.Id);
            Assert.AreEqual(PropertyStage.Listing, studio.Stage);

            studio.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            Assert.AreEqual(PropertyStage.Applications, studio.Stage);
            Assert.Greater(studio.Applicants.Count, 0);

            var applicant = studio.Applicants[0];
            service.SelectApplicant(studio.Id, applicant.Id);
            Assert.AreEqual(PropertyStage.Occupied, studio.Stage);
            Assert.AreEqual(applicant.Name, studio.TenantName);
            Assert.AreEqual(applicant.Role, studio.TenantRole);
            Assert.Greater(studio.Relationship, 0);
        }

        [Test]
        public void BuyToTenantSelection_CompletesBusinessLifecycle()
        {
            var service = NewService();
            var studio = service.State.Properties.Find(property => property.Id == "riverside");

            Assert.IsTrue(service.Buy(studio.Id));
            studio.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            service.ChooseUse(studio.Id, PropertyUse.Business);
            service.PublishListing(studio.Id);
            studio.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();

            Assert.IsTrue(studio.Applicants.TrueForAll(applicant => applicant.IsBusiness));
            Assert.AreEqual("Green Cross Pharmacy", studio.Applicants[0].Name);
        }

        [Test]
        public void RenovationUpgradeNegotiationAndGoals_CreateMeaningfulChoices()
        {
            var service = NewService();
            var starter = service.State.Properties.Find(property => property.Id == "old-town");
            var cash = service.State.Cash;
            Assert.IsTrue(service.Renovate(starter.Id));
            Assert.IsTrue(starter.Renovated);
            Assert.Greater(starter.TenantDailyRent, 620);
            Assert.IsTrue(service.UpgradeProperty(starter.Id));
            Assert.AreEqual(2, starter.Level);
            Assert.IsTrue(service.PromoteProperty(starter.Id));
            Assert.AreEqual(1, starter.Popularity);
            var goal = service.State.Goals.Find(candidate => candidate.Id == "first-upgrade");
            Assert.AreEqual(1, goal.Progress);
            Assert.IsTrue(service.ClaimGoal(goal.Id));
            Assert.Greater(service.State.Cash, cash - 2500);

            var studio = service.State.Properties.Find(property => property.Id == "riverside");
            Assert.IsTrue(service.Buy(studio.Id));
            studio.NotaryCompleteAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            service.ChooseUse(studio.Id, PropertyUse.Residential);
            service.PublishListing(studio.Id);
            studio.ListingAvailableAtUtcTicks = DateTime.UtcNow.AddSeconds(-1).Ticks;
            service.Tick();
            var applicant = studio.Applicants[0];
            var proposedRent = applicant.DailyRent;
            Assert.IsTrue(service.NegotiateApplicant(studio.Id, applicant.Id));
            Assert.Greater(applicant.DailyRent, proposedRent);
            Assert.IsFalse(service.NegotiateApplicant(studio.Id, applicant.Id));
        }

        [Test]
        public void HintSystemAndCompanySkills_ChangeTheNextDecisionAndEconomy()
        {
            var service = NewService();
            var hint = service.GetNextHint();
            Assert.AreEqual("old-town", hint.PropertyId);
            Assert.AreEqual("Collect rent", hint.Action);
            Assert.IsTrue(service.UpgradeCompanySkill("landlord"));
            Assert.AreEqual(1, service.State.LandlordSkill);
            Assert.Greater(service.EffectiveDailyRent(service.State.Properties.Find(property => property.Id == "old-town")), 620);
            Assert.IsTrue(service.UpgradeCompanySkill("lawyer"));
            Assert.AreEqual(1, service.State.LawyerSkill);
            var studio = service.State.Properties.Find(property => property.Id == "riverside");
            Assert.IsTrue(service.Buy(studio.Id));
            Assert.Less(DateTime.UtcNow.AddTicks(10).Ticks, studio.NotaryCompleteAtUtcTicks);
        }

        private static GameService NewService()
        {
            return new GameService(new MemoryRepository(), new GameClock());
        }

        private sealed class MemoryRepository : IGameRepository
        {
            public GameState State;
            public GameState Load() => State;
            public void Save(GameState state) => State = state;
        }
    }
}
