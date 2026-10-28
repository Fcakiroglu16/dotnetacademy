# EF Core 11: Indexing and Querying Nested Complex Types

Sample code for the dotnetacademy.dev lesson
[EF Core 11: Indexing and Querying Nested Complex Types](https://dotnetacademy.dev/lesson/ef-core-11-indexing-and-querying-nested-complex-types/362).

A saved board game is stored in one PostgreSQL table:

- `Board -> Tiles -> Position` is a complex collection in one `jsonb` column.
- `Summary -> Leader / Victory` are nested complex types in ordinary columns,
  indexed with EF Core 11's `HasIndex(g => g.Summary.Leader.PlayerId)`.
- A hand-written migration adds a GIN index on `Board` for `@>` containment
  queries written with `EF.Functions.JsonContains`.

Tested with .NET SDK `11.0.100-rc.2.26504.105`, EF Core `11.0.0-rc.2.26504.105`,
Npgsql EF provider `11.0.0-rc.2` and PostgreSQL 17.

## Layout

| Path | What it is |
| --- | --- |
| `src/NestedGameState.Data` | Model, `GameDbContext`, migrations, queries, seeder |
| `src/NestedGameState.ConsoleApp` | Seeds games, prints SQL, `EXPLAIN ANALYZE` plans and timings |
| `src/NestedGameState.Api` | Minimal API on port 5081 (`NestedGameState.Api.http`) |
| `docker-compose.yml` | PostgreSQL 17 on port 5434 |

## Run

```bash
docker compose up -d --wait
dotnet tool restore

# migrate, seed 20,000 games, print SQL, plans and timings
dotnet run --project src/NestedGameState.ConsoleApp -- all

# or step by step
dotnet run --project src/NestedGameState.ConsoleApp -- reset 1000
dotnet run --project src/NestedGameState.ConsoleApp -- sql
dotnet run --project src/NestedGameState.ConsoleApp -- plans
dotnet run --project src/NestedGameState.ConsoleApp -- bench

dotnet run --project src/NestedGameState.Api --launch-profile http
curl http://localhost:5081/games/led-by/player-042
```

The console app calls `Database.MigrateAsync()` at startup. That is a demo
shortcut; use migration bundles or SQL scripts in production.

Based on Sarah Dutkiewicz's article
[EF Core 11: Querying Deeply Nested Game State Without Schema Friction](https://blog.nimblepros.com/blogs/ef-core-11-querying-deeply-nested-game-state/).
