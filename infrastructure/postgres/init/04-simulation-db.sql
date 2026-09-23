-- EJECUTAR EN LA BASE simulation_db.
-- Tablas para almacenar simulaciones y cuotas generadas por mes.

CREATE TABLE IF NOT EXISTS simulations (
    id SERIAL PRIMARY KEY,
    user_id UUID NOT NULL,
    credit_type_id INTEGER NOT NULL,
    credit_type_name VARCHAR(100) NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    months INTEGER NOT NULL,
    annual_rate NUMERIC(5,2) NOT NULL,
    method VARCHAR(20) NOT NULL,
    total_interest NUMERIC(18,2) NOT NULL,
    total_paid NUMERIC(18,2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS installments (
    id SERIAL PRIMARY KEY,
    simulation_id INTEGER NOT NULL,
    number INTEGER NOT NULL,
    payment NUMERIC(18,2) NOT NULL,
    interest NUMERIC(18,2) NOT NULL,
    principal NUMERIC(18,2) NOT NULL,
    balance NUMERIC(18,2) NOT NULL,
    CONSTRAINT fk_installments_simulation
        FOREIGN KEY (simulation_id)
        REFERENCES simulations(id)
        ON DELETE CASCADE,
    CONSTRAINT uq_installment_number UNIQUE (simulation_id, number)
);

CREATE INDEX IF NOT EXISTS idx_simulations_user_id ON simulations(user_id);
CREATE INDEX IF NOT EXISTS idx_simulations_created_at ON simulations(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_installments_simulation_id ON installments(simulation_id);
