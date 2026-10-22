using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class MembersDemo
{
    public static void Run()
    {
        Demo.Title("3. Members on a union");

        Length a = new Meters(10);
        Length b = new Feet(10);

        Length total = a.Add(b);
        Console.WriteLine($"{a.TotalMeters} m + {b.TotalMeters:F3} m = {total.TotalMeters:F3} m");
        Console.WriteLine(total.Value);            // Meters { Value = 13.048 }
    }
}
