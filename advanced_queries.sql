-- Select first/last names and customer's amount of products
SELECT first_name AS FirstName, last_name AS LastName, 
  (SELECT COUNT(product_id) FROM CustomerCart_Product WHERE customer_cart_id = (SELECT id FROM CustomerCart WHERE customer_id = Customer.id)) AS TotalProducts
FROM Customer;


-- Selecting non-popular products(products that doesn't exist in at least one customer cart)
SELECT P.name AS ProductName FROM Product as P WHERE NOT EXISTS (SELECT * FROM CustomerCart_Product as CP WHERE CP.product_id = P.id);


-- Select seller's products with prices
SELECT P.name AS roductName, SP.price AS ProductPrice FROM product AS P, seller_product AS SP WHERE P.id = SP.product_id AND SP.seller_id = (SELECT id FROM Seller WHERE name = "Seller 2");


-- Select email, phone, address of customers who has orders after 2023-10-15  or order price less or equal 80
SELECT C.email, C.phone, C.address FROM Customer C WHERE C.id IN (
	SELECT customer_id
    FROM `Order`
    WHERE date > '2023-10-15' OR total_amount <= 80
) ORDER BY email limit 100;


-- Select list of distinct customer details for customers who meet the specified criteria(date of order in last 30 days and email has end like @gmail.com)
SELECT DISTINCT C.first_name, C.last_name, C.email FROM Customer as C, `Order` as O WHERE C.id = O.customer_id AND O.date >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) AND C.email LIKE '%@gmail.com';


-- Selecting names of seller whose products have high avg price(bigger than 100)
SELECT S.name AS SellerName FROM Seller S
WHERE (SELECT AVG(SP.price) FROM Seller_Product SP WHERE SP.seller_id = S.id) > 100;


-- Select customers who have orders with one seller
SELECT C.first_name, C.last_name, C.email FROM Customer as C
WHERE (
	SELECT COUNT(DISTINCT SP.seller_id)
	FROM `Order` O, OrderItem OI, Seller_Product SP
    WHERE O.customer_id = C.id AND O.id = OI.order_id AND OI.seller_product_id = SP.id
    ) = 1;


-- Select customers who have placed the most orders
SELECT C.first_name, C.last_name, C.email
FROM Customer as C
WHERE (SELECT COUNT(*) FROM `Order` as O WHERE O.customer_id = C.id) =
      (SELECT MAX(order_count) FROM (SELECT COUNT(*) AS order_count FROM `Order` GROUP BY customer_id) AS Counts);
      
      
--  Retrieve customers and their orders                                
SELECT C.first_name, C.last_name, O.id AS OrderID, O.date FROM Customer C INNER JOIN `Order` O ON C.id = O.customer_id;                                
                                
-- Retrieve sellers and their products                                
SELECT S.name AS SellerName, SP.product_id, SP.price FROM Seller as S INNER JOIN Seller_Product as SP ON S.id = SP.seller_id;

-- Retrieve products with category that contain 'laptop' in description
SELECT P.name AS ProductName, C.name AS CategoryName FROM Product P INNER JOIN Category C ON P.category_id = C.id
			WHERE P.description regexp 'laptop'; -- Filter products with 'laptop' in the description
            
-- Retrieve customers and their orders with condition (returns all customers even if customer doesn't have any orders in that case it passes NULL value)
SELECT C.first_name, C.last_name, O.id AS OrderID, O.date, O.total_amount FROM Customer C LEFT JOIN `Order` O ON C.id = O.customer_id WHERE O.date is null or O.total_amount > 60;


-- Select all customers with order's date before current date
SELECT O.id AS OrderID, C.first_name, C.last_name FROM `Order` O RIGHT JOIN Customer C ON O.customer_id = C.id WHERE O.date <= curdate() order by C.first_name;


-- Select all sellers with seller_products id and price with condition
SELECT S.name AS SellerName, SP.product_id, SP.price AS ProductPrice FROM Seller S LEFT JOIN Seller_Product SP ON S.id = SP.seller_id WHERE SP.price is not null and SP.price > 50;

-- Decart multiply (login with product name: each to each)
SELECT U.login, P.name AS ProductName FROM User U CROSS JOIN Product P;

-- Possible combinations of customer first names and product names
SELECT C.first_name, P.name FROM Customer C CROSS JOIN Product P;


-- Full join(MySql analog using union to combine select results)
-- Customers and their Orders
-- Customers with Orders
SELECT C.first_name, C.last_name, O.id AS OrderID, O.date
FROM Customer C
LEFT JOIN `Order` O ON C.id = O.customer_id

UNION

-- Orders without Customers
SELECT C.first_name, C.last_name, O.id AS OrderID, O.date
FROM Customer C
RIGHT JOIN `Order` O ON C.id = O.customer_id;


-- Calculate total amount spent by Customers
SELECT C.first_name, C.last_name, SUM(O.total_amount) AS TotalSpent FROM Customer C LEFT JOIN `Order` O ON C.id = O.customer_id GROUP BY C.id, C.first_name, C.last_name;

-- Find average product price for each seller
SELECT S.name AS SellerName, AVG(SP.price) AS AveragePrice FROM Seller S LEFT JOIN Seller_Product SP ON S.id = SP.seller_id GROUP BY S.id, S.name;

-- Count the number of products in each category
SELECT C.name AS CategoryName, COUNT(P.id) AS ProductCount FROM Category C LEFT JOIN Product P ON C.id = P.category_id GROUP BY C.id;

--  calculate the average price for each category of products
SELECT
  P.name AS ProductName,
  C.name AS CategoryName,
  SP.price,
  AVG(SP.price) OVER (PARTITION BY C.name) AS AvgPricePerCategory
FROM Product P
INNER JOIN Seller_Product SP ON P.id = SP.product_id
INNER JOIN Category C ON P.category_id = C.id;

-- Find customers who have made more than one order
SELECT C.id, C.first_name, C.last_name, Count(O.id) AS TotalOrders FROM Customer C LEFT JOIN `Order` O ON C.id = O.customer_id group by C.id, C.first_name having TotalOrders > 0;
