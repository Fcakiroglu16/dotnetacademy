#!/usr/bin/env bash
# kubectl port-forward exits after one broken connection (for example when NGINX reloads
# or a data-plane Pod is replaced). This keeps the lab ports open by restarting it.
#   localhost:8080 -> NGF  public-nginx :80     localhost:8443 -> NGF public-nginx :443 (once it exists)
#   localhost:8081 -> Traefik            :80    (if Traefik is installed)
set -u

ngf_ports() {
  # Port 443 only appears on the Service after the HTTPS listener is added.
  if kubectl get svc public-nginx -n pedalshare-infra -o jsonpath='{.spec.ports[*].port}' 2>/dev/null | grep -qw 443; then
    echo "8080:80 8443:443"
  else
    echo "8080:80"
  fi
}

ngf() {
  while true; do
    ports=$(ngf_ports)
    kubectl port-forward -n pedalshare-infra svc/public-nginx $ports >/dev/null 2>&1 &
    pf=$!
    # Restart when the forward dies or the Service gains or loses a port.
    while kill -0 "$pf" 2>/dev/null && [ "$(ngf_ports)" = "$ports" ]; do sleep 2; done
    kill "$pf" 2>/dev/null
    sleep 1
  done
}

traefik() {
  while true; do
    if kubectl get svc traefik -n traefik >/dev/null 2>&1; then
      kubectl port-forward -n traefik svc/traefik 8081:80 >/dev/null 2>&1
    fi
    sleep 1
  done
}

ngf &
traefik &
echo "Forwarding NGF on 8080 (8443 once HTTPS exists) and Traefik on 8081 (if installed). Ctrl+C to stop."
trap 'kill 0' INT TERM
wait
