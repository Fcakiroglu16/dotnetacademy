# C# 15 Union Types

Source code for the dotnetacademy.dev lesson **C# 15 Union Types**.

A union type holds exactly one value from a closed set of case types:

```csharp
public union Pet(Cat, Dog, Bird);

Pet pet = new Dog("Rex", "Husky");      // implicit conversion

string text = pet switch                 // exhaustive, no default arm
{
    Dog d => d.Breed,
    Cat c => $"{c.Lives} lives",
    Bird b => b.Name,
};
```

## Requirements

- .NET 11 SDK, RC 2 or later (`global.json` pins `11.0.100-rc.2`, rolls forward to newer feature bands)
- C# 15 is the default language version for `net11.0`, no `LangVersion` needed

## Projects

| Project | What it shows |
| --- | --- |
| `src/UnionTypes.ConsoleApp` | Declaration, conversions, exhaustive switch, result-or-error, members on a union, value-type and generic cases, `default`/`null`/`IUnion`, a hand-written `[Union]` type, System.Text.Json |
| `src/UnionTypes.Api` | Minimal API with unions in request and response bodies, mapping a result union to HTTP status codes, OpenAPI `anyOf` schemas |

## Run the console app

```bash
dotnet run --project src/UnionTypes.ConsoleApp
```

## Run the API

```bash
dotnet run --project src/UnionTypes.Api
```

The API listens on `http://localhost:5080`. Send the requests in `src/UnionTypes.Api/UnionTypes.Api.http` from Rider, Visual Studio or VS Code, or use curl:

```bash
curl http://localhost:5080/pets
curl -X POST http://localhost:5080/pets -H "Content-Type: application/json" -d '{"name":"Tekir","lives":9}'
curl -X POST http://localhost:5080/payments -H "Content-Type: application/json" -d '{"cardNumber":"4111111111110000","amount":150}'
curl http://localhost:5080/openapi/v1.json
```
