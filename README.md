# EF Core 11 migrations for teams

Sample code for the dotnetacademy.dev lesson. A veterinary clinic model shows four EF Core 11 migration features on SQLite:

- `ExcludeForeignKeyFromMigrations()` for a lab feed that can arrive before its parent rows
- `.config/dotnet-ef.json` so `dotnet ef` needs no `--project`, `--startup-project` or `--context`
- a model snapshot that records the last migration id, so diverged branches conflict in Git
- `--context "*"` across several DbContexts

Requires the .NET 11 SDK (RC 2 or later) and the local tool manifest in `.config/dotnet-tools.json` (EF Core 11.0.0-rc.3).

```bash
dotnet tool restore
dotnet ef database update --context "*"   # applies ClinicDbContext and ScheduleDbContext
dotnet run --project src/Clinic.ConsoleApp
```

The databases are created in `<temp dir>/clinic-lesson/`, so `dotnet run` and `dotnet ef` always see the same files.
