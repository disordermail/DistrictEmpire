using System;

namespace DistrictEmpire.Domain
{
    public readonly struct CountryCode
    {
        public CountryCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 2)
                throw new ArgumentException("Country code must use ISO 3166-1 alpha-2 format.", nameof(value));

            Value = value.ToUpperInvariant();
        }

        public string Value { get; }

        public override string ToString() { return Value; }
    }
}
