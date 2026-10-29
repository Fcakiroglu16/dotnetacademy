# PostgreSQL Connection Pooling with Npgsql and EF Core

Source code for the dotnetacademy.dev lesson [PostgreSQL Connection Pooling in .NET: Preventing Connection Exhaustion](https://dotnetacademy.dev/lesson/postgresql-connection-pooling-in-net-preventing-connection-exhaustion-with-npgsql-and-ef-core/363).

A greenhouse telemetry API runs as several instances against a deliberately small PostgreSQL server
(`max_connections=40`, 3 slots reserved for superusers). The scripts reproduce the production failures
and their fixes: too many instances for the server, retries that hide it, a leaked `DbContext` connection,
a transaction held open across a slow HTTP call, and a correctly budgeted pool.

## Requirements

- .NET 10 SDK (`global.json` pins `10.0.100` and rolls forward)
- Docker (PostgreSQL 17 on port 5435)
- bash (the helper scripts)

## What is in the repository

| Path | What it shows |
| --- | --- |
| `src/GreenhouseTelemetry.Api` | Minimal API. Pool settings in the connection string passed to `UseNpgsql`, `AddPooledDbContextFactory` plus a scoped context, optional `EnableRetryOnFailure`, `Application Name` per instance |
| `src/GreenhouseTelemetry.Api/AlertEndpoints.cs` | A transaction held open across an SMS call, and the fix |
| `src/GreenhouseTelemetry.Api/DiagnosticsEndpoints.cs` | A factory-created `DbContext` that is never disposed after `OpenConnectionAsync`, and the fixes |
| `src/GreenhouseTelemetry.LoadTest` | Concurrent load generator that round-robins over instances and groups errors |
| `scripts/` | Start/stop N instances with a given `Max Pool Size`, run load, snapshot `pg_stat_activity` |
| `db/init.sql` | `telemetry_app` role, schema, 60,000 seeded readings |

## Run it

```bash
docker compose up -d --wait
dotnet build

scripts/start-instances.sh 3 20          # 3 x 20 = 60 possible connections > 37 slots
scripts/load.sh GET '/greenhouses/{n}/climate-report' 120 600 3
scripts/stop-instances.sh

RETRY=true scripts/start-instances.sh 3 20   # same, with EnableRetryOnFailure(3)
scripts/load.sh GET '/greenhouses/{n}/climate-report' 120 600 3
scripts/stop-instances.sh

scripts/start-instances.sh 3 10          # 3 x 10 = 30, fits the budget
scripts/load.sh GET '/greenhouses/{n}/climate-report' 120 600 3
scripts/snapshot.sh                      # run in a second terminal during the load
scripts/stop-instances.sh
```

Clean up with `docker compose down -v`.
