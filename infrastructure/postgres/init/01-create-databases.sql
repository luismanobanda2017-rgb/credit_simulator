-- Este script se ejecuta en la base maestra de PostgreSQL.
-- Crea las tres bases independientes del proyecto.

SELECT 'CREATE DATABASE auth_db OWNER postgres'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'auth_db')\gexec;

SELECT 'CREATE DATABASE credit_catalog_db OWNER postgres'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'credit_catalog_db')\gexec;

SELECT 'CREATE DATABASE simulation_db OWNER postgres'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'simulation_db')\gexec;
