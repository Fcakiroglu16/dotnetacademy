using System.Text.Json;
using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class JsonDemo
{
    public static void Run()
    {
        Demo.Title("7. System.Text.Json and unions");

        // The active case is written as-is: no wrapper, no $type discriminator.
        Console.WriteLine(JsonSerializer.Serialize<IntOrString>(42));            // 42
        Console.WriteLine(JsonSerializer.Serialize<IntOrString>("hello"));       // "hello"
        Console.WriteLine(JsonSerializer.Serialize(new Pet(new Cat("Misty", 7)))); // {"Name":"Misty","Lives":7}
        Console.WriteLine(JsonSerializer.Serialize(default(Pet)));               // null

        // Reading back: the JSON token kind (number/string) selects the case.
        IntOrString n = JsonSerializer.Deserialize<IntOrString>("123");
        IntOrString s = JsonSerializer.Deserialize<IntOrString>("\"abc\"");
        Console.WriteLine($"{n.Value?.GetType().Name}: {n.Value}, {s.Value?.GetType().Name}: {s.Value}");

        // Two object cases: [JsonUnion] with the structural classifier decides by property names.
        string[] payloads =
        [
            """{"To":"ali@example.com","Subject":"Welcome"}""",
            """{"PhoneNumber":"+905551112233","Text":"Your code is 1234"}""",
        ];
        foreach (var json in payloads)
        {
            Notification notification = JsonSerializer.Deserialize<Notification>(json);
            string sent = notification switch
            {
                EmailNotification e => $"email to {e.To}: {e.Subject}",
                SmsNotification sms => $"sms to {sms.PhoneNumber}: {sms.Text}",
            };
            Console.WriteLine(sent);
        }

        // Without a classifier, Pet(Cat, Dog, Bird) can't pick an object case.
        try
        {
            JsonSerializer.Deserialize<Pet>("""{"Name":"Rex","Breed":"Husky"}""");
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        }
    }
}
