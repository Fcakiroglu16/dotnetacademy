-- Who holds the server's connection slots right now?
SELECT application_name, state, count(*) AS connections
FROM pg_stat_activity
WHERE backend_type = 'client backend'
GROUP BY application_name, state
ORDER BY application_name, state;

-- Background processes (autovacuum, checkpointer...) do not use max_connections slots.
SELECT count(*) AS client_connections,
       current_setting('max_connections')::int AS max_connections,
       current_setting('superuser_reserved_connections')::int AS reserved_for_superusers
FROM pg_stat_activity
WHERE backend_type = 'client backend';
