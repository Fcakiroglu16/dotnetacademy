# dotnetacademy

Source code for the lessons on [dotnetacademy.dev](https://dotnetacademy.dev).

Each lesson lives on its own branch. `main` only holds this index.

## C# 15 (.NET 11)

| Lesson | Branch |
| --- | --- |
| [Union Types](https://dotnetacademy.dev/lesson/c-15-union-types-one-value-a-closed-set-of-types/359) | [`csharp15-union-types`](https://github.com/Fcakiroglu16/dotnetacademy/tree/csharp15-union-types) |

## Entity Framework Core

| Lesson | Branch |
| --- | --- |
| [EF Core Migrations the Right Way: Migration Bundles in CI/CD](https://dotnetacademy.dev/lesson/ef-core-migrations-the-right-way-migration-bundles-in-cicd/360) | [`efcore-migration-bundles`](https://github.com/Fcakiroglu16/dotnetacademy/tree/efcore-migration-bundles) |

## Claude Code

| Lesson | Branch |
| --- | --- |
| [Claude Code Permissions for .NET Projects: Allow, Ask and Deny](https://dotnetacademy.dev/lesson/claude-code-permissions-for-net-projects-allow-ask-and-deny/361) | [`claude-code-dotnet-permissions`](https://github.com/Fcakiroglu16/dotnetacademy/tree/claude-code-dotnet-permissions) |

## Running a lesson

```bash
git clone https://github.com/Fcakiroglu16/dotnetacademy.git
cd dotnetacademy
git switch csharp15-union-types
dotnet run --project src/UnionTypes.ConsoleApp
```

The EF Core lessons need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and Docker. The C# 15 lessons need the [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0) (RC 2 or later).
