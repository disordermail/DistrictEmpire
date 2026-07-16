using System;

namespace DistrictEmpire.Domain
{
    public readonly struct Money
    {
        public Money(long minorUnits, string currencyCode)
        {
            if (minorUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(minorUnits), "Money cannot be negative.");

            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required.", nameof(currencyCode));

            MinorUnits = minorUnits;
            CurrencyCode = currencyCode.ToUpperInvariant();
        }

        public long MinorUnits { get; }

        public string CurrencyCode { get; }
    }
}
