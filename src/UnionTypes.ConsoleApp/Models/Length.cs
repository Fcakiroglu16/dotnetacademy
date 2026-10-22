namespace UnionTypes.ConsoleApp.Models;

public record class Meters(double Value);
public record class Feet(double Value);

// A union can declare members, but no instance fields or auto-properties.
public union Length(Meters, Feet)
{
    public double TotalMeters => this switch
    {
        Meters m => m.Value,
        Feet f => f.Value * 0.3048,
        null => throw new InvalidOperationException("The Length has no value."),
    };

    public Length Add(Length other) => new Meters(TotalMeters + other.TotalMeters);
}
