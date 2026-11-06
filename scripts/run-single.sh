#!/usr/bin/env bash
# Topology A: one process, quotes calculated in-process.
set -euo pipefail
cd "$(dirname "$0")/.."

HOSTINGSTARTUPASSEMBLIES="Logistics.Module.Parcels;Logistics.Module.Quotes.Local" \
  dotnet run --project src/Logistics.Host --urls http://localhost:5200
