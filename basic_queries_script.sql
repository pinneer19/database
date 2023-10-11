							-- basic crud operations --
-- Get all products --
SELECT * FROM Product;

-- Get all orders for specific customer using id --
SELECT * FROM `Order` WHERE customer_id = '<customer_id>';

-- Get all categories --
SELECT * FROM Category;

-- Add new product --
INSERT INTO Product (category_id, name, description, price) VALUES ('<category_id>', '<product_name>', '<product_description>', '<product_price>');

-- Add new customer order --
INSERT INTO `Order` (customer_id, date, total_amount) VALUES ('<customer_id>', 'YYYY-MM-DD', '<total_amount>');

-- Update product data --
UPDATE Product SET name = '<new_product_name>', description = '<new_product_description>', price = '<new_price>' WHERE id = '<product_id>';

-- Update order data --
UPDATE `Order` SET date = '<new_order_date>', total_amount = '<new_total_amount>' WHERE id = '<order_id>';

-- Delete product by id --
DELETE FROM Product WHERE id = '<product_id>';

-- Delete order by id --
DELETE FROM `Order` WHERE id = '<order_id>';

-- Select seller_product items with price 599.99 or 19.99 
select * from seller_product where price regexp '599.99|19.99';


-- Select seller_id's with number of rows and grouping by seller_id
select seller_id, count(*) from seller_product group by seller_id;

-- Get amount of users--
select count(*) from user;

-- Select seller_product items with price 599.99 or 19.99 
select * from seller_product where price regexp '599.99|19.99';

-- Select seller_id's with number of rows and grouping by seller_id
select seller_id, count(*) from seller_product group by seller_id;

-- Select customer's first_name if phone starts with 123
select first_name from customer where phone like '123%';

-- Find the total amount for each seller --
select seller_id, SUM(price) as total_revenue from seller_product group by seller_id;

-- Number of sold products by each seller -- 
select seller_id, count(product_id) as num_products_sold from seller_product group by seller_id;

select * from `order` where date >= '2023-10-15' and (customer_id = 1 or total_amount < 60); 

select * from customer where id between 1 and 100 limit 10;


