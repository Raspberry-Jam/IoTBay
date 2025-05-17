/*
 BASE TYPES
 TODO: Fix ON DELETE rules to set default instead of set null
 TODO: Set up fallback defaults for tables that may be dropped but referencing records need preserving
 */
 
 DROP TABLE IF EXISTS contacts CASCADE;
CREATE TABLE contacts
(
    contact_id SERIAL PRIMARY KEY,
    given_name VARCHAR(128) NOT NULL,
    surname VARCHAR(128),
    email VARCHAR(128) NOT NULL,
    phone_number CHAR(10)
);

DROP TABLE IF EXISTS addresses CASCADE;
CREATE TABLE addresses
(
    address_id SERIAL PRIMARY KEY,
    street_line_1 VARCHAR(128),
    street_line_2 VARCHAR(128),
    suburb VARCHAR(128),
    state VARCHAR(3) CHECK (state in ('act', 'nsw', 'nt', 'qld', 'sa', 'tas', 'vic', 'wa')), 
    postcode CHAR(4),
    CONSTRAINT check_address_is_valid_or_null CHECK (
        -- Check that if any of the fields are filled, then all required ones are filled.
        -- StreetLine2 is not required, but if it is filled then the rest are required.
        (street_line_1 IS NOT NULL AND street_line_2 IS NOT NULL AND suburb IS NOT NULL AND state IS NOT NULL AND
         postcode IS NOT NULL) OR
        (street_line_1 IS NOT NULL AND suburb IS NOT NULL AND state IS NOT NULL AND postcode IS NOT NULL) OR
            -- Also if all are empty, it is fine.
        (street_line_1 IS NULL AND street_line_2 IS NULL AND suburb IS NULL AND state IS NULL AND postcode IS NULL)
        )
);

DROP TABLE IF EXISTS users CASCADE;
CREATE TABLE users
(
    user_id SERIAL PRIMARY KEY,
    password_hash CHAR(256) NOT NULL,
    password_salt CHAR(128) NOT NULL,
    contact_id INT NOT NULL UNIQUE,
    FOREIGN KEY (contact_id) REFERENCES contacts (contact_id) ON DELETE SET NULL
);
/* TODO: Add fallback user for other tables to reference upon user deletion */
/* and a System user for admin operations. */

DROP TABLE IF EXISTS user_access_events CASCADE;
CREATE TABLE user_access_events
(
    user_access_event_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    event_time TIMESTAMP NOT NULL,
    event_type VARCHAR(8) NOT NULL CHECK (event_type IN ('login', 'logout')),
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL
);

DROP TABLE IF EXISTS products CASCADE;
CREATE TABLE products
(
    product_id SERIAL PRIMARY KEY,
    name VARCHAR(256) NOT NULL UNIQUE,
    type VARCHAR(128) NOT NULL,
    price FLOAT, /* Nullable for items not for sale */
    stock INT NOT NULL,
    short_description VARCHAR(512),
    full_description TEXT
);

DROP TABLE IF EXISTS shipment_methods CASCADE;
CREATE TABLE shipment_methods
(
    shipment_method_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    address_id INT NOT NULL,
    method VARCHAR(128) NOT NULL CHECK (method IN ('auspost', 'dhl', 'fedex')), 
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (address_id) REFERENCES addresses (address_id) ON DELETE SET NULL
);

DROP TABLE IF EXISTS payment_methods CASCADE;
CREATE TABLE payment_methods
(
    payment_method_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    card_number CHAR(16) NOT NULL, /* TODO: Validate Luhn digit to check for errors */
    CVV CHAR(3) NOT NULL,
    expiry DATE NOT NULL, /* TODO: Ensure to retrieve MM/YY in application*/
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL
);

DROP TABLE IF EXISTS orders CASCADE;
CREATE TABLE orders
(
    order_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    shipment_method_id INT NOT NULL,
    payment_method_id INT NOT NULL,
    order_date DATE NOT NULL,
    sent_date DATE, /* Nullable as this is set some time after the order is made */
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (shipment_method_id) REFERENCES shipment_methods (shipment_method_id) ON DELETE SET NULL,
    FOREIGN KEY (payment_method_id) REFERENCES payment_methods (payment_method_id) ON DELETE SET NULL
);

DROP TABLE IF EXISTS order_products CASCADE;
CREATE TABLE order_products
(
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders (order_id) ON DELETE SET DEFAULT,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE SET DEFAULT
);

DROP TABLE IF EXISTS user_cart_products CASCADE;
CREATE TABLE user_cart_products
(
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (user_id, product_id),
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE SET NULL
);