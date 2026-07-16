using System;
using System.Collections.Generic;

namespace DistrictEmpire.Domain
{
    public enum PropertyUse { None, Residential, Business }
    public enum PropertyStage { Occupied, Available, Notary, ChoosingUse, Listing, Applications }

    [Serializable]
    public sealed class Applicant
    {
        public string Id;
        public string Name;
        public string Role;
        public string Story;
        public int DailyRent;
        public bool IsBusiness;
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
        public int Condition;
        public bool IsOwned;
        public PropertyStage Stage;
        public PropertyUse Use;
        public string TenantName;
        public int TenantDailyRent;
        public long NotaryCompleteAtUtcTicks;
        public long ListingAvailableAtUtcTicks;
        public List<Applicant> Applicants = new();
    }

    [Serializable]
    public sealed class GameState
    {
        public int Cash = 32000;
        public int Influence = 35;
        public int Day = 1;
        public int RentReady;
        public long LastClockUtcTicks;
        public List<Property> Properties = new();
    }
}
