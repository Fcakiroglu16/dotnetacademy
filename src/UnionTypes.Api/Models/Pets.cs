using System.Text.Json.Serialization;

namespace UnionTypes.Api.Models;

public record class Cat(string Name, int Lives);
public record class Dog(string Name, string Breed);
public record class Bird(string Name, bool CanTalk);

// All three cases are JSON objects; the structural classifier picks the case
// from the property names in the request body (lives / breed / canTalk).
[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union Pet(Cat, Dog, Bird);

public record class PetEntry(int Id, Pet Pet);
