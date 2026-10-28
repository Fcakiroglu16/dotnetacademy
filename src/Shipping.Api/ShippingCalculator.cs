namespace Shipping.Api;

public static class ShippingCalculator
{
    public static decimal Quote(decimal weightKg, bool express)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weightKg);

        var price = 5m + weightKg * 1.5m;
        return express ? price * 2 : price;
    }
}
