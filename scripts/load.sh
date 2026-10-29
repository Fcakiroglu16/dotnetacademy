#!/usr/bin/env bash
# usage: scripts/load.sh <method> <path> <concurrency> <requests> <instances>
set -euo pipefail
urls=(); for i in $(seq 1 "$5"); do urls+=("http://localhost:$((5090 + i))"); done
dotnet src/GreenhouseTelemetry.LoadTest/bin/Debug/net10.0/GreenhouseTelemetry.LoadTest.dll "$1" "$2" "$3" "$4" "${urls[@]}"
