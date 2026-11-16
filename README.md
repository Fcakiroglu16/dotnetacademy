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
| [EF Core 11: Indexing and Querying Nested Complex Types](https://dotnetacademy.dev/lesson/ef-core-11-indexing-and-querying-nested-complex-types/362) | [`efcore11-nested-complex-indexes`](https://github.com/Fcakiroglu16/dotnetacademy/tree/efcore11-nested-complex-indexes) |
| [PostgreSQL Connection Pooling in .NET: Preventing Connection Exhaustion with Npgsql and EF Core](https://dotnetacademy.dev/lesson/postgresql-connection-pooling-in-net-preventing-connection-exhaustion-with-npgsql-and-ef-core/363) | [`postgres-connection-pooling`](https://github.com/Fcakiroglu16/dotnetacademy/tree/postgres-connection-pooling) |
| [EF Core 11 Migrations: Constraint-Free Relationships, Snapshot Conflicts and the dotnet-ef Config File](https://dotnetacademy.dev/lesson/ef-core-11-migrations-constraint-free-relationships-snapshot-conflicts-and-the-dotnet-ef-config-file/365) | [`efcore11-migrations-for-teams`](https://github.com/Fcakiroglu16/dotnetacademy/tree/efcore11-migrations-for-teams) |

## Claude Code

| Lesson | Branch |
| --- | --- |
| [Claude Code Permissions for .NET Projects: Allow, Ask and Deny](https://dotnetacademy.dev/lesson/claude-code-permissions-for-net-projects-allow-ask-and-deny/361) | [`claude-code-dotnet-permissions`](https://github.com/Fcakiroglu16/dotnetacademy/tree/claude-code-dotnet-permissions) |

## Modular Monolith

| Lesson | Branch |
| --- | --- |
| [Modular Monolith in ASP.NET Core with IHostingStartup: Choose Your Modules at Startup](https://dotnetacademy.dev/lesson/modular-monolith-in-aspnet-core-with-ihostingstartup-choose-your-modules-at-startup/364) | [`aspnet-modular-monolith`](https://github.com/Fcakiroglu16/dotnetacademy/tree/aspnet-modular-monolith) |

## Kubernetes Gateway API

Branches are cumulative: each one builds on the one before it. Run `scripts/cluster-up.sh` (kind, Docker, Helm).

| Lesson | Branch |
| --- | --- |
| [Kubernetes Gateway API Fundamentals: GatewayClass, Gateway, Routes and Status Conditions](https://dotnetacademy.dev/lesson/kubernetes-gateway-api-fundamentals-gatewayclass-gateway-routes-and-status-conditions/366) | [`gateway-api-fundamentals`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-fundamentals) |
| [Gateway API with NGINX Gateway Fabric: Installation, Routing and NGINX Extensions](https://dotnetacademy.dev/lesson/gateway-api-with-nginx-gateway-fabric-installation-routing-and-nginx-extensions/367) | [`gateway-api-nginx-gateway-fabric`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-nginx-gateway-fabric) |
| [Gateway API with Traefik: Running the Same Routes on a Second Implementation](https://dotnetacademy.dev/lesson/gateway-api-with-traefik-running-the-same-routes-on-a-second-implementation/368) | [`gateway-api-traefik`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-traefik) |
| [HTTPRoute Matching and Filters: Headers, Query Parameters, Redirects, Rewrites and Mirroring](https://dotnetacademy.dev/lesson/httproute-matching-and-filters-headers-query-parameters-redirects-rewrites-and-mirroring/369) | [`gateway-api-httproute-matching`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-httproute-matching) |
| [HTTPS with Gateway API and cert-manager: Automatic Certificates and HTTP to HTTPS Redirects](https://dotnetacademy.dev/lesson/https-with-gateway-api-and-cert-manager-automatic-certificates-and-http-to-https-redirects/370) | [`gateway-api-https-cert-manager`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-https-cert-manager) |
| [Canary and Blue/Green Releases with Gateway API Traffic Weights](https://dotnetacademy.dev/lesson/canary-and-bluegreen-releases-with-gateway-api-traffic-weights/371) | [`gateway-api-traffic-splitting`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-traffic-splitting) |
| [Sharing a Gateway Across Namespaces: ReferenceGrant and GRPCRoute for .NET gRPC](https://dotnetacademy.dev/lesson/sharing-a-gateway-across-namespaces-referencegrant-and-grpcroute-for-net-grpc/372) | [`gateway-api-referencegrant-grpcroute`](https://github.com/Fcakiroglu16/dotnetacademy/tree/gateway-api-referencegrant-grpcroute) |

## Running a lesson

```bash
git clone https://github.com/Fcakiroglu16/dotnetacademy.git
cd dotnetacademy
git switch csharp15-union-types
dotnet run --project src/UnionTypes.ConsoleApp
```

The EF Core lessons need Docker and the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (the EF Core 11 lesson needs .NET 11). The C# 15 lessons need the [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0) (RC 2 or later).
