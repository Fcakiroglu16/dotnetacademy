using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class CustomUnionDemo
{
    public static void Run()
    {
        Demo.Title("6. A hand-written [Union] type");

        Shape[] shapes = [new Circle(5), new Rectangle(3, 4)];
        foreach (var shape in shapes)
        {
            double area = shape switch
            {
                Circle c => Math.PI * c.Radius * c.Radius,
                Rectangle r => r.Width * r.Height,
            };
            Console.WriteLine($"{shape.Value!.GetType().Name,-9} area = {area:F2}");
        }
    }
}
