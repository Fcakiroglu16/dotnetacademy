#!/usr/bin/env bash
# Prints pg_stat_activity as the superuser, which still gets in when the app slots are full.
docker compose exec -T postgres psql -U postgres -d greenhouse -f - < scripts/connections.sql
