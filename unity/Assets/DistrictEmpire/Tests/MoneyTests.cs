using System;
using DistrictEmpire.Domain;
using NUnit.Framework;

namespace DistrictEmpire.Tests
{
    public sealed class MoneyTests
    {
        [Test]
        public void Constructor_NormalizesCurrencyCode()
        {
            var money = new Money(100, "pln");

            Assert.AreEqual("PLN", money.CurrencyCode);
        }

        [Test]
        public void Constructor_RejectsNegativeAmounts()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1, "PLN"));
        }
    }
}
