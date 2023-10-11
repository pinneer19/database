-- Insert test data into Role table
INSERT INTO Role (name) VALUES ('admin');
INSERT INTO Role (name) VALUES ('seller');
INSERT INTO Role (name) VALUES ('customer');

-- Insert test data into User table
INSERT INTO User (role_id, login, password) VALUES (1, 'adminuser', 'adminpass');
INSERT INTO User (role_id, login, password) VALUES (2, 'selleruser', 'sellerpass');
INSERT INTO User (role_id, login, password) VALUES (3, 'customeruser', 'customerpass');

-- Insert test data into Seller table
INSERT INTO Seller (user_id, name) VALUES (2, 'Seller 1');
INSERT INTO Seller (user_id, name) VALUES (2, 'Seller 2');

-- Insert test data into Customer table
INSERT INTO Customer (user_id, first_name, last_name, email, phone, address) VALUES
		(3, 'John', 'Doe', 'john@example.com', '123456789', '123 Main St');
INSERT INTO Customer (user_id, first_name, last_name, email, phone, address) VALUES
		(3, 'Jane', 'Smith', 'jane@example.com', '987654321', '456 Elm St');

-- Insert test data into Admin table
INSERT INTO Admin (user_id) VALUES (1);

-- Insert test data into Category table
INSERT INTO Category (name) VALUES ('Electronics');
INSERT INTO Category (name) VALUES ('Clothing');
INSERT INTO Category (name) VALUES ('Books');

-- Insert test data into Product table
INSERT INTO Product (category_id, name, description) VALUES (1, 'Laptop', 'High-performance laptop');
INSERT INTO Product (category_id, name, description) VALUES (1, 'Smartphone', 'Latest smartphone model');
INSERT INTO Product (category_id, name, description) VALUES (2, 'T-shirt', 'Cotton T-shirt');
INSERT INTO Product (category_id, name, description) VALUES (2, 'Jeans', 'Blue jeans');
INSERT INTO Product (category_id, name, description) VALUES (3, 'Novel', 'Best-selling novel');

-- Insert test data into Seller_Product table
INSERT INTO Seller_Product (product_id, seller_id, price) VALUES (1, 1, 999.99);
INSERT INTO Seller_Product (product_id, seller_id, price) VALUES (2, 1, 599.99);
INSERT INTO Seller_Product (product_id, seller_id, price) VALUES (3, 2, 19.99);
INSERT INTO Seller_Product (product_id, seller_id, price) VALUES (4, 2, 39.99);
INSERT INTO Seller_Product (product_id, seller_id, price) VALUES (5, 2, 9.99);

-- Insert test data into `Order` table
INSERT INTO `Order` (customer_id, date, total_amount) VALUES (1, '2023-10-15', 79.92); -- 8 novels --
INSERT INTO `Order` (customer_id, date, total_amount) VALUES (2, '2023-10-16', 59.98); -- 1 jeans and t-shirt -- 

-- Insert test data into Card table
INSERT INTO Card (customer_id, card_number) VALUES (1, '1234-5678-9012-3456');
INSERT INTO Card (customer_id, card_number) VALUES (2, '9876-5432-1098-7654');

-- Insert test data into CustomerCart table
INSERT INTO CustomerCart (customer_id, product_quantity) VALUES (1, 1);
INSERT INTO CustomerCart (customer_id, product_quantity) VALUES (2, 2);

-- Insert test data into CustomerCart_Product table
INSERT INTO CustomerCart_Product (customer_cart_id, product_id, total_amount) VALUES (1, 5, 8);
INSERT INTO CustomerCart_Product (customer_cart_id, product_id, total_amount) VALUES (2, 3, 1);
INSERT INTO CustomerCart_Product (customer_cart_id, product_id, total_amount) VALUES (2, 4, 1);

-- Insert test data into OrderItem table
INSERT INTO OrderItem (seller_product_id, order_id, total_amount) VALUES (5, 1, 8);
INSERT INTO OrderItem (seller_product_id, order_id, total_amount) VALUES (3, 2, 1);
INSERT INTO OrderItem (seller_product_id, order_id, total_amount) VALUES (4, 2, 1);

-- Insert test data into Log table
INSERT INTO Log (action, user_id, datetime) VALUES ('Login', 1, '2023-10-15 09:00:00');
INSERT INTO Log (action, user_id, datetime) VALUES ('Logout', 1, '2023-10-15 17:30:00');