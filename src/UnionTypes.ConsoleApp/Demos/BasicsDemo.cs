using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class BasicsDemo
{
    public static void Run()
    {
        Demo.Title("1. Declaration and implicit conversion");

        // Every case type converts to the union implicitly.
        Pet pet = new Dog("Rex", "Husky");
        Console.WriteLine(pet.Value);              // Dog { Name = Rex, Breed = Husky }

        pet = new Cat("Whiskers", 9);
        Console.WriteLine(pet.Value);              // Cat { Name = Whiskers, Lives = 9 }

        // The compiler emits a struct, so Pet is a value type.
        Console.WriteLine(typeof(Pet).IsValueType); // True

        Pet[] pets = [new Dog("Rex", "Husky"), new Cat("Misty", 7), new Bird("Kiwi", true)];
        foreach (var p in pets)
        {
            Console.WriteLine(Describe(p));
        }
    }

    // Exhaustive switch: no discard arm needed. Remove the Bird arm and the
    // compiler warns that the switch doesn't handle every case type.
    public static string Describe(Pet pet) => pet switch
    {
        Dog d => $"{d.Name} is a {d.Breed}",
        Cat c => $"{c.Name} has {c.Lives} lives left",
        Bird b => b.CanTalk ? $"{b.Name} can talk" : $"{b.Name} only sings",
    };
}
