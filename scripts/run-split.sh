#!/usr/bin/env bash
# Topology B: same build, two processes. Gateway (5200) calls the quotes backend (5201) over HTTP.
set -euo pipefail
cd "$(dirname "$0")/.."

dotnet build src/Logistics.Host -v q
HOST_DLL=src/Logistics.Host/bin/Debug/net10.0/Logistics.Host.dll

HOSTINGSTARTUPASSEMBLIES="Logistics.Module.Quotes.Api" \
  dotnet "$HOST_DLL" --urls http://localhost:5201 &
BACKEND_PID=$!
trap 'kill $BACKEND_PID' EXIT

HOSTINGSTARTUPASSEMBLIES="Logistics.Module.Parcels;Logistics.Module.Quotes.Remote" \
QuoteService__BaseUrl="http://localhost:5201" \
  dotnet "$HOST_DLL" --urls http://localhost:5200
