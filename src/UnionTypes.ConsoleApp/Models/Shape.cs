using System.Runtime.CompilerServices;

namespace UnionTypes.ConsoleApp.Models;

public record class Circle(double Radius);
public record class Rectangle(double Width, double Height);

// A hand-written union: [Union] + one single-parameter constructor per case + Value.
[Union]
public struct Shape : IUnion
{
    private readonly object? _value;

    public Shape(Circle value) => _value = value;
    public Shape(Rectangle value) => _value = value;

    public object? Value => _value;
}
