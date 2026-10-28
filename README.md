# EF Core 11: Indexing and Querying Nested Complex Types

Sample code for the dotnetacademy.dev lesson
[EF Core 11: Indexing and Querying Nested Complex Types](https://dotnetacademy.dev/lesson/ef-core-11-indexing-and-querying-nested-complex-types/362).

A parcel carrier stores shipments in one PostgreSQL table:

- `Plan -> Stops -> Address` is a complex collection in one `jsonb` column.
- `Tracking -> Carrier / LastScan` are nested complex types in ordinary columns,
  indexed with EF Core 11's `HasIndex(s => s.Tracking.LastScan.HubCode)`.
- The origin country (`Stops[0]`) is copied out of the JSON into a stored
  generated column with a normal index.
- A hand-written migration adds a GIN index on `Plan` for `@>` containment
  queries written with `EF.Functions.JsonContains`.

Tested with .NET SDK `11.0.100-rc.2.26504.105`, EF Core `11.0.0-rc.2.26504.105`,
Npgsql EF provider `11.0.0-rc.2` and PostgreSQL 17.

## Layout

| Path | What it is |
| --- | --- |
| `src/ShipmentTracking.Data` | Model, `ShippingDbContext`, migrations, queries, seeder |
| `src/ShipmentTracking.ConsoleApp` | Seeds shipments, prints SQL, `EXPLAIN ANALYZE` plans and timings |
| `src/ShipmentTracking.Api` | Minimal API on port 5081 (`ShipmentTracking.Api.http`) |
| `docker-compose.yml` | PostgreSQL 17 on port 5434 |

## Run

```bash
docker compose up -d --wait
dotnet tool restore

# migrate, seed 20,000 shipments, print SQL, plans and timings
dotnet run --project src/ShipmentTracking.ConsoleApp -- all

# or step by step
dotnet run --project src/ShipmentTracking.ConsoleApp -- reset 1000
dotnet run --project src/ShipmentTracking.ConsoleApp -- sql
dotnet run --project src/ShipmentTracking.ConsoleApp -- plans
dotnet run --project src/ShipmentTracking.ConsoleApp -- bench

dotnet run --project src/ShipmentTracking.Api --launch-profile http
curl http://localhost:5081/shipments/customs/Basel/count
```

The console app calls `Database.MigrateAsync()` at startup. That is a demo
shortcut; use migration bundles or SQL scripts in production.

The topic was inspired by Sarah Dutkiewicz's article
[EF Core 11: Querying Deeply Nested Game State Without Schema Friction](https://blog.nimblepros.com/blogs/ef-core-11-querying-deeply-nested-game-state/).
The scenario and code here are different.
