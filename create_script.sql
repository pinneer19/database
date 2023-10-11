create database online_store;

use online_store;

CREATE TABLE Role (
	id bigint auto_increment primary key,
    name varchar(30) not null
);

CREATE TABLE User (
    id BIGINT PRIMARY KEY auto_increment,
    role_id BIGINT,
    login VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    FOREIGN KEY (role_id) REFERENCES Role(id) on delete cascade
);

CREATE TABLE Seller (
    id BIGINT PRIMARY KEY auto_increment,
    user_id BIGINT,
    name VARCHAR(255) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES User(id) on delete cascade
);

CREATE TABLE Customer (
    id BIGINT PRIMARY KEY auto_increment,
    user_id BIGINT,
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(255) NOT NULL,
    address VARCHAR(255) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES User(id) on delete cascade
);
CREATE TABLE Admin (
    id BIGINT PRIMARY KEY auto_increment,
    user_id BIGINT,
    FOREIGN KEY (user_id) REFERENCES User(id) on delete cascade
);
CREATE TABLE Category (
    id BIGINT PRIMARY KEY auto_increment,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE Product (
    id BIGINT PRIMARY KEY auto_increment,
    category_id BIGINT,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(255) NOT NULL,
    FOREIGN KEY (category_id) REFERENCES Category(id) on delete cascade
);

CREATE TABLE Seller_Product (
    id BIGINT PRIMARY KEY auto_increment,
    product_id BIGINT,
    seller_id BIGINT,
    price FLOAT NOT NULL,
    FOREIGN KEY (product_id) REFERENCES Product(id) on delete cascade,
    FOREIGN KEY (seller_id) REFERENCES Seller(id) on delete cascade
);

CREATE TABLE `Order` (
    id BIGINT PRIMARY KEY auto_increment,
    customer_id BIGINT,
    date DATE NOT NULL,
    total_amount FLOAT NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES Customer(id) on delete cascade
);

CREATE TABLE Card (
    id BIGINT PRIMARY KEY auto_increment,
    customer_id BIGINT UNIQUE,
    card_number VARCHAR(255) NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES Customer(id) on delete cascade
);

CREATE TABLE CustomerCart (
    id BIGINT PRIMARY KEY auto_increment,
    customer_id BIGINT UNIQUE,
    product_quantity INT NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES Customer(id) on delete cascade
);

CREATE TABLE CustomerCart_Product (
    id BIGINT PRIMARY KEY auto_increment,
    customer_cart_id BIGINT NOT NULL,
    product_id BIGINT,
    total_amount INT NOT NULL,
    FOREIGN KEY (customer_cart_id) REFERENCES CustomerCart(id) on delete cascade,
    FOREIGN KEY (product_id) REFERENCES Product(id) on delete cascade
);

CREATE TABLE OrderItem (
    id BIGINT PRIMARY KEY auto_increment,
    seller_product_id BIGINT,
    order_id BIGINT,
    total_amount FLOAT NOT NULL,
    FOREIGN KEY (seller_product_id) REFERENCES Seller_Product(id) on delete cascade,
    FOREIGN KEY (order_id) REFERENCES `Order`(id) on delete cascade
);

CREATE TABLE Log (
    id BIGINT PRIMARY KEY auto_increment,
    action VARCHAR(255) NOT NULL,
    user_id BIGINT,
    datetime DATETIME NOT NULL,
    FOREIGN KEY (user_id) REFERENCES User(id) on delete cascade
);