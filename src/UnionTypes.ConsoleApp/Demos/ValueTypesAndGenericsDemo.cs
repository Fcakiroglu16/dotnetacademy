using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class ValueTypesAndGenericsDemo
{
    public static void Run()
    {
        Demo.Title("4. Value-type cases and generic unions");

        IntOrString[] values = [42, "forty-two"];
        foreach (var value in values)
        {
            string text = value switch
            {
                int i => $"int    -> {i * 2}",
                string s => $"string -> {s.ToUpperInvariant()}",
            };
            Console.WriteLine(text);
        }

        // int cases are boxed: Value is object?.
        IntOrString number = 7;
        Console.WriteLine(number.Value?.GetType());  // System.Int32

        Option<string> found = new Some<string>("dotnetacademy.dev");
        Option<string> missing = new None();
        Console.WriteLine(Show(found));
        Console.WriteLine(Show(missing));

        static string Show(Option<string> option) => option switch
        {
            Some<string> some => $"Some({some.Value})",
            None => "None",
        };
    }
}
