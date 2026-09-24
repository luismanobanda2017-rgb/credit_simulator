-- Base independiente para la auditoría de intentos de acceso.
\connect login_audit_db

CREATE TABLE IF NOT EXISTS login_attempts (
    id BIGSERIAL PRIMARY KEY,
    user_id UUID NULL,
    username VARCHAR(100) NOT NULL,
    registered_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    successful BOOLEAN NOT NULL,
    attempt_number INTEGER NOT NULL
);


CREATE INDEX IF NOT EXISTS idx_login_attempts_username ON login_attempts(username);
CREATE INDEX IF NOT EXISTS idx_login_attempts_registered_at ON login_attempts(registered_at);