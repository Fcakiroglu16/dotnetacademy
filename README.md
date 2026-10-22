# dotnetacademy

Source code for the lessons on [dotnetacademy.dev](https://dotnetacademy.dev).

Each lesson lives on its own branch. `main` only holds this index.

## C# 15 (.NET 11)

| Lesson | Branch |
| --- | --- |
| Union Types | [`csharp15-union-types`](https://github.com/Fcakiroglu16/dotnetacademy/tree/csharp15-union-types) |

## Running a lesson

```bash
git clone https://github.com/Fcakiroglu16/dotnetacademy.git
cd dotnetacademy
git switch csharp15-union-types
dotnet run --project src/UnionTypes.ConsoleApp
```

The C# 15 lessons need the [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0) (RC 2 or later).
