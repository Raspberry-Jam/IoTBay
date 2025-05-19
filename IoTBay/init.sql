/*
 BASE TYPES
 
 Format:
    - If the table exists, delete it and any other tables that reference it
    - Create the table
    - Set its primary key default to -1 (fallback value)
    - Insert a default fallback row
 */

DROP TABLE IF EXISTS contacts CASCADE;
CREATE TABLE contacts
(
    contact_id   SERIAL PRIMARY KEY,
    given_name   VARCHAR(128) NOT NULL,
    surname      VARCHAR(128),
    email        VARCHAR(128) NOT NULL,
    phone_number CHAR(10)
);
INSERT INTO contacts (contact_id, given_name, email)
VALUES (-1, 'DEFAULT', 'DEFAULT');

DROP TABLE IF EXISTS addresses CASCADE;
CREATE TABLE addresses
(
    address_id    SERIAL PRIMARY KEY,
    street_line_1 VARCHAR(128),
    street_line_2 VARCHAR(128),
    suburb        VARCHAR(128),
    state         VARCHAR(3) CHECK (state in ('act', 'nsw', 'nt', 'qld', 'sa', 'tas', 'vic', 'wa')),
    postcode      CHAR(4),
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
INSERT INTO addresses (address_id)
VALUES (-1);

DROP TABLE IF EXISTS users CASCADE;
CREATE TABLE users
(
    user_id       SERIAL PRIMARY KEY,
    password_hash CHAR(256) NOT NULL,
    password_salt CHAR(128) NOT NULL,
    contact_id    INT       NOT NULL UNIQUE,
    role VARCHAR(8) NOT NULL CHECK (role IN ('customer', 'staff', 'system')),
    FOREIGN KEY (contact_id) REFERENCES contacts (contact_id) ON DELETE SET DEFAULT
);
INSERT INTO users (user_id, password_hash, password_salt, contact_id, role)
VALUES (-1, 'INVALID_HASH', 'INVALID_SALT', -1, 'customer');

DROP TABLE IF EXISTS user_access_events CASCADE;
CREATE TABLE user_access_events
(
    user_access_event_id SERIAL PRIMARY KEY,
    user_id              INT        NOT NULL,
    event_time           TIMESTAMP  NOT NULL,
    event_type           VARCHAR(8) NOT NULL CHECK (event_type IN ('login', 'logout', 'unknown')),
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE -- Delete this row if the user is deleted
);
INSERT INTO user_access_events (user_access_event_id, user_id, event_time, event_type)
VALUES (-1, -1, '0001-01-01 00:00:00', 'unknown');

DROP TABLE IF EXISTS products CASCADE;
CREATE TABLE products
(
    product_id        SERIAL PRIMARY KEY,
    name              VARCHAR(256) NOT NULL UNIQUE,
    type              VARCHAR(128) NOT NULL,
    price             FLOAT, -- Nullable for items not for sale
    stock             INT          NOT NULL CHECK (stock >= 0),
    short_description VARCHAR(512),
    full_description  TEXT
);
INSERT INTO products (product_id, name, type, stock)
VALUES (-1, 'INVALID', 'INVALID', 0);

DROP TABLE IF EXISTS shipment_methods CASCADE;
CREATE TABLE shipment_methods
(
    shipment_method_id SERIAL PRIMARY KEY,
    user_id            INT          NOT NULL,
    address_id         INT          NOT NULL,
    method             VARCHAR(128) NOT NULL CHECK (method IN ('auspost', 'dhl', 'fedex', 'unknown')),
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET DEFAULT,
    FOREIGN KEY (address_id) REFERENCES addresses (address_id) ON DELETE SET DEFAULT
);
INSERT INTO shipment_methods (shipment_method_id, user_id, address_id, method)
VALUES (-1, -1, -1, 'unknown');

DROP TABLE IF EXISTS payment_methods CASCADE;
CREATE TABLE payment_methods
(
    payment_method_id SERIAL PRIMARY KEY,
    user_id           INT      NOT NULL,
    card_number       CHAR(16) NOT NULL,
    cvv               CHAR(3)  NOT NULL,
    expiry            DATE     NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET DEFAULT
);
INSERT INTO payment_methods (payment_method_id, user_id, card_number, cvv, expiry)
VALUES (-1, -1, 'INVALID', 'INV', '0001-01-01');

DROP TABLE IF EXISTS orders CASCADE;
CREATE TABLE orders
(
    order_id           SERIAL PRIMARY KEY,
    user_id            INT  NOT NULL,
    shipment_method_id INT  NOT NULL,
    payment_method_id  INT  NOT NULL,
    order_date         DATE NOT NULL,
    sent_date          DATE, /* Nullable as this is set some time after the order is made */
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (shipment_method_id) REFERENCES shipment_methods (shipment_method_id) ON DELETE SET DEFAULT,
    FOREIGN KEY (payment_method_id) REFERENCES payment_methods (payment_method_id) ON DELETE SET DEFAULT
);
INSERT INTO orders (order_id, user_id, shipment_method_id, payment_method_id, order_date)
VALUES (-1, -1, -1, -1, '0001-01-01');

DROP TABLE IF EXISTS order_products CASCADE;
CREATE TABLE order_products
(
    order_id   INT NOT NULL,
    product_id INT NOT NULL,
    quantity   INT NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders (order_id) ON DELETE SET DEFAULT,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE SET DEFAULT
);

DROP TABLE IF EXISTS user_cart_products CASCADE;
CREATE TABLE user_cart_products
(
    user_id    INT NOT NULL,
    product_id INT NOT NULL,
    quantity   INT NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (user_id, product_id),
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE SET DEFAULT,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE SET DEFAULT
);

/* Default System account. Unhashed password is "SuperSecretAdminPassword" */
INSERT INTO contacts (contact_id, given_name, email)
VALUES (-2, 'System', 'admin@iotbay.com');

INSERT INTO users (user_id, password_hash, password_salt, contact_id, role)
VALUES (-2,
        '981501C5BAAF270B23B271A2FCF9439EE9FC8CA2491FCC2551163EF7317AF1C26D23EE82FA294694A50759293B5BEDB573F65E4EC8623164CC585FB38245DF2837048EF6547975415CE914899EDDFFA878F920E24D34CAE41FB3ACB2AB4B41CEEA8EFA55A7A4C61A71DA0820C1A0818E470C3BD1F5A8E1E2158FA6E60E1E922B',
        '6C70084AEE2D5E0C927BE07A49E6F25FBCA0C8A52BEA6B524634C2D06667C2648CD164583D9AD415BA0F64EF6AC417F301C6B6BDD877958899B295A7424AD5E3',
        -2,
        'system');

/* Ensure that all automatic primary key increments are explicitly set to begin at 1 */
ALTER SEQUENCE contacts_contact_id_seq RESTART WITH 1;
ALTER SEQUENCE addresses_address_id_seq RESTART WITH 1;
ALTER SEQUENCE users_user_id_seq RESTART WITH 1;
ALTER SEQUENCE user_access_events_user_access_event_id_seq RESTART WITH 1;
ALTER SEQUENCE products_product_id_seq RESTART WITH 1;
ALTER SEQUENCE shipment_methods_shipment_method_id_seq RESTART WITH 1;
ALTER SEQUENCE payment_methods_payment_method_id_seq RESTART WITH 1;
ALTER SEQUENCE orders_order_id_seq RESTART WITH 1;