-- This script runs automatically when the Postgres container starts for the first time.
-- It creates a separate database for each service (database-per-service pattern).
-- Each service owns its data completely — no service ever queries another service's database.

CREATE DATABASE auth_db;
CREATE DATABASE user_db;
CREATE DATABASE order_db;
CREATE DATABASE inventory_db;
CREATE DATABASE payment_db;
CREATE DATABASE notification_db;
