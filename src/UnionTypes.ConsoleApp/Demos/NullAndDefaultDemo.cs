using System.Runtime.CompilerServices;
using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class NullAndDefaultDemo
{
    public static void Run()
    {
        Demo.Title("5. default, null and IUnion");

        // A default struct union holds no case.
        Pet empty = default;
        Console.WriteLine(empty.Value is null);     // True

        Console.WriteLine(DescribeOrNone(empty));   // no pet
        Console.WriteLine(DescribeOrNone(new Dog("Rex", "Husky")));

        // Without a null arm, a default union throws at run time.
        try
        {
            Console.WriteLine(BasicsDemo.Describe(empty));
        }
        catch (SwitchExpressionException)
        {
            Console.WriteLine("SwitchExpressionException: default(Pet) reached a switch without a null arm");
        }

        // Nullable union: Pet? is Nullable<Pet>.
        Pet? maybe = null;
        Console.WriteLine(maybe is null);           // True

        // Every union implements IUnion, so you can inspect any union at run time.
        object boxed = new Pet(new Bird("Kiwi", true));
        if (boxed is IUnion { Value: { } inner })
        {
            Console.WriteLine($"IUnion holds a {inner.GetType().Name}");
        }
    }

    private static string DescribeOrNone(Pet pet) => pet switch
    {
        Dog d => d.Name,
        Cat c => c.Name,
        Bird b => b.Name,
        null => "no pet",
    };
}
