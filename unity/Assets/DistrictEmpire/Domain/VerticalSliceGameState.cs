using System;
using System.Collections.Generic;

namespace DistrictEmpire.Domain
{
    public enum PropertyUse { None, Residential, Business }
    public enum PropertyStage { Occupied, Available, Notary, ChoosingUse, Listing, Applications, CancellingContract, ForSale }

    [Serializable]
    public sealed class CityEvent
    {
        public string Id;
        public string Title;
        public string Detail;
        public string Reward;
        public bool Claimed;
    }

    [Serializable]
    public sealed class NpcActivity
    {
        public string Investor;
        public string Title;
        public string Detail;
    }

    [Serializable]
    public sealed class CompanyGoal
    {
        public string Id;
        public string Title;
        public string Detail;
        public int Target;
        public int Progress;
        public bool Claimed;
    }

    [Serializable]
    public sealed class Applicant
    {
        public string Id;
        public string Name;
        public string Role;
        public string Story;
        public int DailyRent;
        public bool IsBusiness;
        public bool Negotiated;
    }

    [Serializable]
    public sealed class Property
    {
        public string Id;
        public string Name;
        public string District;
        public string Icon;
        public int Price;
        public int BaseDailyRent;
        public int Tier;
        public string Category;
        public float MapX;
        public float MapY;
        public int Condition;
        public int Level = 1;
        public int Popularity;
        public bool Renovated;
        public bool IsOwned;
        public PropertyStage Stage;
        public PropertyUse Use;
        public string TenantName;
        public string TenantRole;
        public string TenantStory;
        public int Relationship;
        public int TenantDailyRent;
        public string BuildingName;
        public int BuildingOwnedUnits;
        public int BuildingTotalUnits;
        public long NotaryCompleteAtUtcTicks;
        public long ListingAvailableAtUtcTicks;
        public long ContractEndAtUtcTicks;
        public int SalePrice;
        public List<Applicant> Applicants = new();
    }

    [Serializable]
    public sealed class GameState
    {
        public int Cash = 32000;
        public int Influence = 35;
        public int Xp;
        public int CompanyLevel = 1;
        public int LandlordSkill;
        public int LawyerSkill;
        public bool DailyRewardClaimed;
        public int Day = 1;
        public int EventsDay;
        public int RentReady;
        public long LastClockUtcTicks;
        public List<Property> Properties = new();
        public List<CityEvent> Events = new();
        public List<NpcActivity> NpcActivities = new();
        public List<CompanyGoal> Goals = new();
    }
}
