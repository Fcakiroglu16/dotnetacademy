using Shipping.Api;

namespace Shipping.Tests;

public class ShippingCalculatorTests
{
    [Theory]
    [InlineData(2, false, 8)]
    [InlineData(2, true, 16)]
    public void Quote_returns_base_price_plus_weight(decimal weightKg, bool express, decimal expected) =>
        Assert.Equal(expected, ShippingCalculator.Quote(weightKg, express));

    [Fact]
    public void Quote_rejects_zero_weight() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ShippingCalculator.Quote(0, false));
}
