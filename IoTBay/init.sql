/*
 BASE TYPES
 */

DROP TABLE IF EXISTS contacts CASCADE;
CREATE TABLE contacts
(
    contact_id   SERIAL PRIMARY KEY,
    given_name   VARCHAR(128) NOT NULL,
    surname      VARCHAR(128),
    email        VARCHAR(128) NOT NULL,
    phone_number VARCHAR(10)
);

DROP TYPE IF EXISTS state_enum CASCADE;
CREATE TYPE state_enum AS ENUM ('ACT', 'NSW', 'NT', 'QLD', 'SA', 'TAS', 'VIC', 'WA');

DROP TABLE IF EXISTS addresses CASCADE;
CREATE TABLE addresses
(
    address_id    SERIAL PRIMARY KEY,
    street_line_1 VARCHAR(128),
    street_line_2 VARCHAR(128),
    suburb        VARCHAR(128),
    state         state_enum,
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

DROP TABLE IF EXISTS users CASCADE;
CREATE TABLE users
(
    user_id       SERIAL PRIMARY KEY,
    username      VARCHAR(64)  NOT NULL UNIQUE,
    password_hash VARCHAR(256) NOT NULL,
    password_salt VARCHAR(128) NOT NULL,
    contact_id    INT          NOT NULL UNIQUE,
    address_id    INT,
    FOREIGN KEY (contact_id) REFERENCES contacts (contact_id) ON DELETE SET NULL,
    FOREIGN KEY (address_id) REFERENCES addresses (address_id) ON DELETE SET NULL
);

DROP TYPE IF EXISTS permission_enum CASCADE;
CREATE TYPE permission_enum AS ENUM ('clerk', 'manager', 'admin');

DROP TABLE IF EXISTS staff CASCADE;
CREATE TABLE staff
(
    staff_id   SERIAL PRIMARY KEY,
    user_id    INT             NOT NULL UNIQUE,
    permission permission_enum NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS stores CASCADE;
CREATE TABLE stores
(
    store_id   SERIAL PRIMARY KEY,
    contact_id INT NOT NULL,
    address_id INT NOT NULL,
    FOREIGN KEY (contact_id) REFERENCES contacts (contact_id) ON DELETE SET NULL,
    FOREIGN KEY (address_id) REFERENCES addresses (address_id) ON DELETE SET NULL
);

DROP TABLE IF EXISTS catalogues CASCADE;
CREATE TABLE catalogues
(
    catalogue_id SERIAL PRIMARY KEY
);

DROP TABLE IF EXISTS products CASCADE;
CREATE TABLE products
(
    product_id         SERIAL PRIMARY KEY,
    name               VARCHAR(256) NOT NULL UNIQUE,
    price              FLOAT,
    short_description  VARCHAR(512),
    full_description   TEXT, -- TEXT storage type utilises the "Over-Sized Attribute" storage optimisation
    thumbnail_uri      VARCHAR(128),
    gallery_folder_uri VARCHAR(128)
);

DROP TABLE IF EXISTS orders CASCADE;
CREATE TABLE orders
(
    order_id   SERIAL PRIMARY KEY,
    user_id    INT NOT NULL,
    store_id   INT,                                                                -- Null if being delivered
    address_id INT,                                                                -- Null if being picked up from store
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE,            -- If the user is deleted, delete all their orders
    FOREIGN KEY (store_id) REFERENCES stores (store_id) ON DELETE SET NULL,        -- If the store is deleted, retain order history
    FOREIGN KEY (address_id) REFERENCES addresses (address_id) ON DELETE SET NULL, -- If the address is deleted, retain order history
    CONSTRAINT check_delivery_or_pickup CHECK (
        (store_id IS NOT NULL AND address_id IS NULL) OR
        (store_id IS NULL AND address_id IS NOT NULL)
        )
);

DROP TABLE IF EXISTS carts CASCADE;
CREATE TABLE carts
(
    cart_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE CASCADE
);

/*
 ASSOCIATIVE TYPES (MANY-TO-MANY)
 */
DROP TABLE IF EXISTS store_catalogues CASCADE;
CREATE TABLE store_catalogues
(
    store_id     INT NOT NULL,
    catalogue_id INT NOT NULL,
    PRIMARY KEY (store_id, catalogue_id),
    FOREIGN KEY (store_id) REFERENCES stores (store_id) ON DELETE CASCADE,
    FOREIGN KEY (catalogue_id) REFERENCES catalogues (catalogue_id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS catalogue_product_list CASCADE;
CREATE TABLE catalogue_product_list
(
    catalogue_id INT NOT NULL,
    product_id   INT NOT NULL,
    PRIMARY KEY (catalogue_id, product_id),
    FOREIGN KEY (catalogue_id) REFERENCES catalogues (catalogue_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS store_product_stock CASCADE;
CREATE TABLE store_product_stock
(
    store_id   INT NOT NULL,
    product_id INT NOT NULL,
    stock      INT NOT NULL CHECK (stock >= 0), -- Ensure non-negative values
    PRIMARY KEY (store_id, product_id),
    FOREIGN KEY (store_id) REFERENCES stores (store_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS order_products CASCADE;
CREATE TABLE order_products
(
    order_id   INT NOT NULL,
    product_id INT NOT NULL,
    amount     INT NOT NULL CHECK (amount > 0), -- Ensure positive values
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders (order_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS cart_products CASCADE;
CREATE TABLE cart_products
(
    cart_id    INT NOT NULL,
    product_id INT NOT NULL,
    amount     INT NOT NULL CHECK (amount > 0), -- Ensure positive values
    PRIMARY KEY (cart_id, product_id),
    FOREIGN KEY (cart_id) REFERENCES carts (cart_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (product_id) ON DELETE CASCADE
);
