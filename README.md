# EF Core Migration Bundles

Source code for the dotnetacademy.dev lesson **EF Core Migrations the Right Way: Migration Bundles in CI/CD**.

Instead of calling `MigrateAsync()` when the app starts, the schema is changed by a separate,
versioned executable (`efbundle`) that runs with its own database identity:

```bash
dotnet ef migrations bundle --project src/MigrationBundles.Data --startup-project src/MigrationBundles.Api \
  --context CatalogDbContext --output artifacts/catalog-bundle

./artifacts/catalog-bundle --connection "$MIGRATION_CONNECTION_STRING"
```

## Requirements

- .NET 10 SDK (`global.json` pins `10.0.100` and rolls forward to newer feature bands)
- Docker (PostgreSQL 17)
- `dotnet-ef` 10.0.14 is pinned in `dotnet-tools.json`; run `dotnet tool restore`

## What is in the repository

| Path | What it shows |
| --- | --- |
| `src/MigrationBundles.Data` | `CatalogDbContext` (3 migrations: InitialCreate, AddProductSku, AddProductStock), `AuditDbContext` in its own `audit` schema with its own history table, `AuditDbContextFactory` (`IDesignTimeDbContextFactory`), idempotent `UseSeeding`/`UseAsyncSeeding` |
| `src/MigrationBundles.Api` | Minimal API on port 5080. Migrates on startup only in Development (hybrid approach), `/health/schema` returns 503 while migrations are pending |
| `db/init.sql` | Two identities: `shop_migrator` owns the schema, `shop_app` can only read and write data |
| `deploy/Dockerfile.migrator`, `deploy/migrate.sh` | Self-contained bundles in a `runtime-deps` image |
| `deploy/k8s` | PostgreSQL and a one-shot migration `Job` |
| `.github/workflows/migrations.yml` | Build job (model check, SQL review scripts, bundles) and a migrate job that only gets the artifact |

## Run it

```bash
docker compose up -d --wait
dotnet tool restore

# Production-like database "shop": build the bundles and apply them as shop_migrator
export ASPNETCORE_ENVIRONMENT=Production
dotnet ef migrations bundle --project src/MigrationBundles.Data --startup-project src/MigrationBundles.Api \
  --context CatalogDbContext --configuration Release --output artifacts/catalog-bundle --force
dotnet ef migrations bundle --project src/MigrationBundles.Data --startup-project src/MigrationBundles.Data \
  --context AuditDbContext --configuration Release --output artifacts/audit-bundle --force
cp src/MigrationBundles.Api/appsettings.json artifacts/

MIG="Host=localhost;Port=5433;Database=shop;Username=shop_migrator;Password=migrator_pw"
(cd artifacts && ./catalog-bundle --connection "$MIG" && ./audit-bundle --connection "$MIG")

# Run the API as shop_app (no DDL rights)
APP="Host=localhost;Port=5433;Database=shop;Username=shop_app;Password=app_pw"
ConnectionStrings__Catalog="$APP" Audit__ConnectionString="$APP" ASPNETCORE_URLS=http://localhost:5080 \
  dotnet run --no-launch-profile --project src/MigrationBundles.Api
```

Then send the requests in `src/MigrationBundles.Api/MigrationBundles.Api.http`.

`dotnet run --project src/MigrationBundles.Api` without the overrides uses the Development
profile: it migrates the local `shop_dev` database on startup.

## Migrator image and Kubernetes Job

```bash
docker build -f deploy/Dockerfile.migrator -t shop-migrator:1.0.0 .

kubectl create namespace ef-bundles-demo
kubectl -n ef-bundles-demo create configmap postgres-init --from-file=init.sql=db/init.sql
kubectl -n ef-bundles-demo apply -f deploy/k8s/postgres.yaml
kubectl -n ef-bundles-demo rollout status deploy/postgres
kubectl -n ef-bundles-demo create secret generic shop-migrator-db \
  --from-literal=connection-string="Host=postgres;Database=shop;Username=shop_migrator;Password=migrator_pw"
kubectl -n ef-bundles-demo apply -f deploy/k8s/migration-job.yaml
kubectl -n ef-bundles-demo wait --for=condition=complete job/shop-migrate-1-0-0
kubectl -n ef-bundles-demo logs job/shop-migrate-1-0-0
```

The passwords in this repository are local demo values. Use real secrets in any shared environment.
