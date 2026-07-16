using System;

namespace DistrictEmpire.Domain
{
    public readonly struct LocalizationKey
    {
        public LocalizationKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Localization key is required.", nameof(value));

            Value = value;
        }

        public string Value { get; }

        public override string ToString() { return Value; }
    }
}
