-- Two identities: the migrator owns the schema, the application only reads and writes data.
CREATE ROLE shop_migrator LOGIN PASSWORD 'migrator_pw';
CREATE ROLE shop_app LOGIN PASSWORD 'app_pw';

-- shop: production-like database, schema changes only come from the migration bundle.
CREATE DATABASE shop OWNER shop_migrator;
-- shop_dev: local development database, the API migrates it on startup.
CREATE DATABASE shop_dev OWNER shop_migrator;

\connect shop
ALTER SCHEMA public OWNER TO shop_migrator;
GRANT CONNECT ON DATABASE shop TO shop_app;
GRANT USAGE ON SCHEMA public TO shop_app;
-- Everything the migrator creates later is readable/writable by the app, but never alterable.
ALTER DEFAULT PRIVILEGES FOR ROLE shop_migrator GRANT USAGE ON SCHEMAS TO shop_app;
ALTER DEFAULT PRIVILEGES FOR ROLE shop_migrator GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO shop_app;
ALTER DEFAULT PRIVILEGES FOR ROLE shop_migrator GRANT USAGE, SELECT ON SEQUENCES TO shop_app;

\connect shop_dev
ALTER SCHEMA public OWNER TO shop_migrator;
