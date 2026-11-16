#!/usr/bin/env bash
# Sends N requests to the pricing API and counts which version answered.
# Usage: scripts/count-versions.sh [requests] [extra curl args...]
set -uo pipefail
n=${1:-200}; shift || true
for _ in $(seq 1 "$n"); do
  body=$(curl -s -H 'Host: pricing.pedalshare.local' "$@" 'localhost:8080/fares/quote?minutes=25')
  [[ $body =~ \"version\":\"([^\"]+)\" ]] && echo "${BASH_REMATCH[1]}" || echo "error"
done | sort | uniq -c
