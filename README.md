# Kubernetes Gateway API series: PedalShare

Source code for the dotnetacademy.dev lesson [HTTPRoute Matching and Filters: Headers, Query Parameters, Redirects, Rewrites and Mirroring](https://dotnetacademy.dev/lesson/httproute-matching-and-filters-headers-query-parameters-redirects-rewrites-and-mirroring/369) (lesson 4 of 7).

PedalShare is a city bike-sharing platform: a stations API, a rides API, a pricing API and a gRPC fleet
telemetry service, all .NET 10. One echo image (`src/PedalShare.Api`) plays every HTTP service; it returns the
service name, version, Pod, path, query and `X-*` headers it received, so each lesson can show which backend
answered. `src/PedalShare.Telemetry` is the ASP.NET Core gRPC service.

Branches are cumulative: each one contains the files of the lessons before it.

| # | Lesson | Branch | Added files |
| --- | --- | --- | --- |
| 1 | [Kubernetes Gateway API Fundamentals: GatewayClass, Gateway, Routes and Status Conditions](https://dotnetacademy.dev/lesson/kubernetes-gateway-api-fundamentals-gatewayclass-gateway-routes-and-status-conditions/366) | `gateway-api-fundamentals` | `k8s/01-fundamentals` |
| 2 | [Gateway API with NGINX Gateway Fabric: Installation, Routing and NGINX Extensions](https://dotnetacademy.dev/lesson/gateway-api-with-nginx-gateway-fabric-installation-routing-and-nginx-extensions/367) | `gateway-api-nginx-gateway-fabric` | `k8s/02-nginx-gateway-fabric`, `k8s/apps/stations-api.yaml` |
| 3 | [Gateway API with Traefik: Running the Same Routes on a Second Implementation](https://dotnetacademy.dev/lesson/gateway-api-with-traefik-running-the-same-routes-on-a-second-implementation/368) | `gateway-api-traefik` | `k8s/03-traefik` |
| 4 | [HTTPRoute Matching and Filters: Headers, Query Parameters, Redirects, Rewrites and Mirroring](https://dotnetacademy.dev/lesson/httproute-matching-and-filters-headers-query-parameters-redirects-rewrites-and-mirroring/369) | `gateway-api-httproute-matching` | `k8s/04-httproute-matching`, `k8s/apps/rides-api.yaml` |

## Requirements

- Docker, [kind](https://kind.sigs.k8s.io/) v0.33+, kubectl, Helm 3.8+
- curl; `grpcurl` for the gRPC lesson
- .NET 10 SDK only if you want to build the apps outside Docker

| Component | Version |
| --- | --- |
| Kubernetes (kind node image) | v1.36.4 |
| Gateway API CRDs | v1.6.3, Standard channel |
| NGINX Gateway Fabric | 2.8.1 |
| Traefik (chart) | 3.7.18 (42.4.0) |
| cert-manager | 1.21.3 |

## Run it

```bash
git clone https://github.com/Fcakiroglu16/dotnetacademy.git
cd dotnetacademy
git switch gateway-api-httproute-matching

scripts/cluster-up.sh       # kind cluster "gateway-lab", images, Gateway API CRDs
# follow the lesson, then:
scripts/cluster-down.sh
```

`scripts/port-forward.sh` (from lesson 2 on) keeps `localhost:8080`/`8443` open to the NGINX Gateway Fabric
data plane and `localhost:8081` to Traefik, restarting `kubectl port-forward` when it drops.

