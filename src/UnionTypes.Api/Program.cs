using Microsoft.AspNetCore.Http.HttpResults;
using UnionTypes.Api.Models;
using UnionTypes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<PetStore>();
builder.Services.AddSingleton<PaymentService>();

var app = builder.Build();

app.MapOpenApi();

// 1. Union in the response body: each item is written as its active case.
app.MapGet("/pets", (PetStore store) => store.GetAll());

app.MapGet("/pets/{id:int}", Results<Ok<Pet>, NotFound> (int id, PetStore store) =>
    store.Find(id) is { } pet ? TypedResults.Ok(pet) : TypedResults.NotFound());

// 2. Union in the request body: the JSON shape decides Cat, Dog or Bird.
app.MapPost("/pets", Created<PetEntry> (Pet pet, PetStore store) =>
{
    var entry = store.Add(pet);
    return TypedResults.Created($"/pets/{entry.Id}", entry);
});

app.MapGet("/pets/{id:int}/sound", Results<Ok<string>, NotFound> (int id, PetStore store) =>
{
    if (store.Find(id) is not { } pet)
    {
        return TypedResults.NotFound();
    }

    // Exhaustive: adding a case to Pet turns this into a compiler warning.
    string sound = pet switch
    {
        Dog d => $"{d.Name}: Woof!",
        Cat c => $"{c.Name}: Meow!",
        Bird b => b.CanTalk ? $"{b.Name}: Hello!" : $"{b.Name}: Tweet!",
    };
    return TypedResults.Ok(sound);
});

// 3. Result-or-error: map every union case to an HTTP response.
app.MapPost("/payments", Results<Ok<PaymentSucceeded>, UnprocessableEntity<PaymentDeclined>, BadRequest<PaymentFailed>>
    (PaymentRequest request, PaymentService payments) =>
        payments.Charge(request) switch
        {
            PaymentSucceeded ok => TypedResults.Ok(ok),
            PaymentDeclined declined => TypedResults.UnprocessableEntity(declined),
            PaymentFailed failed => TypedResults.BadRequest(failed),
        });

// 4. Value-type cases: a number or a status text in the same field.
app.MapGet("/readings", () => new SensorReading[]
{
    new("temperature", 21),
    new("humidity", 48),
    new("pressure", "offline"),
});

app.Run();
