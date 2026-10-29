#!/usr/bin/env bash
# usage: scripts/start-instances.sh <instances> <maxPoolSize> [timeoutSeconds]
set -euo pipefail
count=$1; max_pool=$2; timeout=${3:-15}
dll=src/GreenhouseTelemetry.Api/bin/Debug/net10.0/GreenhouseTelemetry.Api.dll
mkdir -p .run
for i in $(seq 1 "$count"); do
  port=$((5090 + i))
  InstanceName="ingest-$i" Pool__MaxPoolSize="$max_pool" Pool__TimeoutSeconds="$timeout" \
    dotnet "$dll" --contentRoot "$PWD/src/GreenhouseTelemetry.Api" --urls "http://localhost:$port" > ".run/ingest-$i.log" 2>&1 &
  echo $! > ".run/ingest-$i.pid"
  echo "ingest-$i on :$port (Max Pool Size=$max_pool, Timeout=$timeout s)"
done
for i in $(seq 1 "$count"); do
  for _ in $(seq 1 50); do curl -sf "http://localhost:$((5090 + i))/sensors/calibration" > /dev/null && break; sleep 0.3; done
done
