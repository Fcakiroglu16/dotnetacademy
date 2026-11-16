#!/usr/bin/env bash
# Disposable lab cluster for the Gateway API lessons: kind, 1 control plane + 2 workers,
# PedalShare images loaded into the nodes, Gateway API v1.6.3 standard-channel CRDs.
set -euo pipefail
cd "$(dirname "$0")/.."

GATEWAY_API_VERSION=v1.6.3

kind create cluster --name gateway-lab --image kindest/node:v1.36.4 --config k8s/kind-cluster.yaml

docker build -t pedalshare-api:1.0 src/PedalShare.Api
docker build -t pedalshare-telemetry:1.0 src/PedalShare.Telemetry
kind load docker-image pedalshare-api:1.0 pedalshare-telemetry:1.0 --name gateway-lab

kubectl apply --server-side -f \
  "https://github.com/kubernetes-sigs/gateway-api/releases/download/${GATEWAY_API_VERSION}/standard-install.yaml"
