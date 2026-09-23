-- EJECUTAR EN LA BASE credit_catalog_db.
-- Catálogo de tipos de crédito disponibles para la simulación.

CREATE TABLE IF NOT EXISTS credit_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    annual_rate NUMERIC(5,2) NOT NULL CHECK (annual_rate >= 0 AND annual_rate <= 100),
    min_amount NUMERIC(18,2) NOT NULL,
    max_amount NUMERIC(18,2) NOT NULL,
    max_months INTEGER NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

INSERT INTO credit_types (name, annual_rate, min_amount, max_amount, max_months, is_active)
VALUES
    ('Personal', 16.00, 1000.00, 50000.00, 60, TRUE),
    ('Hipotecario', 9.00, 20000.00, 500000.00, 360, TRUE),
    ('Vehicular', 12.00, 5000.00, 120000.00, 84, TRUE),
    ('Educativo', 8.00, 2000.00, 60000.00, 120, TRUE),
    ('PyME', 11.00, 5000.00, 250000.00, 96, TRUE)
ON CONFLICT (name) DO NOTHING;
