#!/usr/bin/env bash
# kubectl port-forward exits after one broken connection (for example when NGINX reloads
# or a data-plane Pod is replaced). This keeps the lab ports open by restarting it.
#   localhost:8080 -> NGF  public-nginx :80     localhost:8443 -> NGF public-nginx :443
#   localhost:8081 -> Traefik            :80
set -u
forward() {
  while true; do
    kubectl port-forward "$@" >/dev/null 2>&1
    sleep 1
  done
}
forward -n pedalshare-infra svc/public-nginx 8080:80 8443:443 &
if kubectl get svc traefik -n traefik >/dev/null 2>&1; then
  forward -n traefik svc/traefik 8081:80 &
fi
echo "Forwarding NGF on 8080/8443 and Traefik on 8081 (if installed). Ctrl+C to stop."
trap 'kill 0' INT TERM
wait
