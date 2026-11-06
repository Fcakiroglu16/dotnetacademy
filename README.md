# Modular monolith in ASP.NET Core with IHostingStartup

Sample code for the dotnetacademy.dev lesson. A parcel-shipping app is split into modules. The host does not reference any module type. The `HOSTINGSTARTUPASSEMBLIES` environment variable decides which modules load, so the same build runs as one process or as two.

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

| Project | Role |
| --- | --- |
| `Logistics.Contracts` | `IShippingQuoteService` and its records |
| `Logistics.Quotes.Core` | price rule shared by the local and API modules |
| `Logistics.ModuleKit` | `ModuleBase` (hosting startup + startup filter) |
| `Logistics.Host` | empty web host, `UseRouting` only |
| `Modules/Parcels` | `GET /parcels/{id}/quote`, consumes the contract |
| `Modules/Quotes.Local` | in-process implementation |
| `Modules/Quotes.Remote` | HTTP client implementation |
| `Modules/Quotes.Api` | `POST /quotes`, the remote side |

## Run

```bash
./scripts/run-single.sh   # one process on :5200
./scripts/run-split.sh    # gateway on :5200, quotes backend on :5201
```

Then call `GET http://localhost:5200/parcels/1/quote` (see `requests.http`). In the split topology `calculatedBy` shows the backend's process id.
