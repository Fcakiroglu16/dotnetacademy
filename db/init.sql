CREATE ROLE telemetry_app LOGIN PASSWORD 'telemetry';
CREATE DATABASE greenhouse OWNER telemetry_app;

\connect greenhouse telemetry_app

CREATE TABLE sensor_readings (
    id            bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    greenhouse_id int          NOT NULL,
    sensor_code   text         NOT NULL,
    temperature_c numeric(5,2) NOT NULL,
    humidity_pct  numeric(5,2) NOT NULL,
    recorded_at   timestamptz  NOT NULL
);
CREATE INDEX ix_sensor_readings_greenhouse_time ON sensor_readings (greenhouse_id, recorded_at);

CREATE TABLE climate_alerts (
    id            bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    greenhouse_id int         NOT NULL,
    message       text        NOT NULL,
    raised_at     timestamptz NOT NULL
);

INSERT INTO sensor_readings (greenhouse_id, sensor_code, temperature_c, humidity_pct, recorded_at)
SELECT g, 'GH' || g || '-S' || (i % 8), 18 + random() * 14, 40 + random() * 50, now() - (i || ' seconds')::interval
FROM generate_series(1, 12) AS g, generate_series(1, 5000) AS i;
