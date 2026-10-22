namespace UnionTypes.ConsoleApp.Models;

// Value-type cases are boxed into the object? Value property.
public union IntOrString(int, string);

public record class None;
public record class Some<T>(T Value);

// Generic unions work too.
public union Option<T>(None, Some<T>);
