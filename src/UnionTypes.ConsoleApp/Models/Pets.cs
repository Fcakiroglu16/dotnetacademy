namespace UnionTypes.ConsoleApp.Models;

public record class Cat(string Name, int Lives);
public record class Dog(string Name, string Breed);
public record class Bird(string Name, bool CanTalk);

// A union composes existing types into a closed set of alternatives.
public union Pet(Cat, Dog, Bird);
