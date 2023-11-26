-- DROP TRIGGER IF EXISTS <name>

-- Trigger to create customer cart after inserting new customer
/*  
	CREATE TRIGGER `customer_AFTER_INSERT` AFTER INSERT ON `customer` FOR EACH ROW
	BEGIN
		INSERT INTO CustomerCart (customer_id, product_quantity)
		VALUES (NEW.id, 0);
	END
*/

-- Trigger to increment product quantity in customer cart after inserting new product there
/*
	CREATE TRIGGER `customercart_product_AFTER_INSERT` AFTER INSERT ON `customercart_product` FOR EACH ROW
	BEGIN
		UPDATE CustomerCart
		SET product_quantity = product_quantity + 1
		WHERE id = NEW.customer_cart_id;
	END
*/

-- insert into customer (user_id, email, phone, address) values (3, 'mail', 'phone', 'address');
-- SET @ID = (SELECT MAX(id) FROM customercart);
-- insert into customercart_product (product_id, customer_cart_id, total_amount) values (2, @ID, 3); 
-- select customer_id, email, phone, address, product_quantity from customercart join customer on customer.id = customercart.customer_id;
-- delete from customer where user_id = 3 and email='mail' and phone='phone' and address='address';  

 -- Trigger for clearing customer cart after making an order and creating log
/*
CREATE TRIGGER `order_AFTER_INSERT` AFTER INSERT ON `order` FOR EACH ROW
BEGIN
	INSERT INTO Log (user_id, `action`, `datetime`)
    VALUES (
		(SELECT user_id FROM Customer WHERE id = NEW.customer_id),
        CONCAT('Order Created: ', NEW.id, ' by customer: ', NEW.customer_id, ' on ', NEW.`date`),
        NOW()
    );
END
*/

-- insert into customer (user_id, email, phone, address) values (3, 'mail', 'phone', 'address');
-- set @ID = (SELECT MAX(id) FROM customer);
-- set @CART_ID = (SELECT MAX(id) FROM customercart);
-- insert into customercart_product (product_id, customer_cart_id, total_amount) values (2, @CART_ID, 5);
-- insert into customercart_product (product_id, customer_cart_id, total_amount) values (3, @CART_ID, 2);
-- insert into `order` (customer_id, `date`, total_amount) values (@ID, CURDATE(), 368);
-- delete from `order` where id = (SELECT MAX(id) from `order`);
-- delete from customer where user_id = 3 and email='mail' and phone='phone' and address='address';  

-- DROP PROCEDURE IF EXISTS CreateOrderItem $$
-- DELIMITER $$
-- CREATE PROCEDURE CreateOrderItem(
--     IN p_order_id BIGINT,
--     IN p_seller_product_id BIGINT,
--     IN p_customercart_product_id BIGINT,
--     OUT p_orderitem_id BIGINT
-- )
-- BEGIN

--     DECLARE v_total_amount FLOAT;
-- 	SET v_total_amount =  (SELECT price from seller_product WHERE p_seller_product_id = id) *
-- 						(SELECT total_amount from customercart_product WHERE p_customercart_product_id = id);
--                         
-- 	INSERT INTO OrderItem (seller_product_id, order_id, total_amount)
--     VALUES (p_seller_product_id, p_order_id, v_total_amount);
--     
-- 	SELECT LAST_INSERT_ID() INTO p_orderitem_id;
-- END $$
-- DELIMITER ;


-- insert into customer (user_id, email, phone, address) values (3, 'mail', 'phone', 'address');
-- set @ID = (SELECT MAX(id) FROM customer);
-- set @CART_ID = (SELECT MAX(id) FROM customercart);

-- insert into customercart_product (product_id, customer_cart_id, total_amount) values (2, @CART_ID, 5);
-- set @CS_PRODUCT_ID = (SELECT MAX(id) FROM customercart_product);
-- insert into `order` (customer_id, `date`, total_amount) values (@ID, CURDATE(), 360);
-- CALL CreateOrderItem((SELECT max(id) from `order`), 3, @CS_PRODUCT_ID, @newOrderItemID);

-- delete from `order` where id = (SELECT MAX(id) from `order`);
-- delete from customer where user_id = 3 and email='mail' and phone='phone' and address='address';  

-- DROP PROCEDURE IF EXISTS CreateOrderItem $$
-- DELIMITER $$
-- CREATE PROCEDURE ClearCustomercart(
--     IN p_customer_id BIGINT
-- )
-- BEGIN
--     DELETE FROM customercart_product WHERE customer_cart_id = (
-- 		SELECT id FROM customercart
--         WHERE customer_id = p_customer_id
--     );
-- END $$
-- DELIMITER ;

/*
CREATE TRIGGER `order_AFTER_INSERT` AFTER INSERT ON `order` FOR EACH ROW
BEGIN
	DELETE FROM customercart_product
    WHERE customer_cart_id = (
		SELECT id FROM customercart
        WHERE customer_id = NEW.customer_id
    );    
	INSERT INTO Log (user_id, `action`, `datetime`)
    VALUES (
		(SELECT user_id FROM Customer WHERE id = NEW.customer_id),
        CONCAT('Order Created: ', NEW.id, ' by customer: ', NEW.customer_id, ' on ', NEW.`date`),
        NOW()
    );
END
*/
-- DELIMITER //
-- CREATE TRIGGER `sellerproduct_AFTER_INSERT` AFTER INSERT ON seller_product FOR EACH ROW
-- BEGIN
-- 	INSERT INTO Log (user_id, `action`, `datetime`)
--     VALUES (
-- 		(SELECT user_id FROM Seller WHERE id = NEW.seller_id),
--         CONCAT('Seller item Created: ', NEW.id, ' by seller: ', NEW.seller_id, ' for price: ', NEW.price),
--         NOW()
--     );
-- END //
-- DELIMITER ;

-- DROP PROCEDURE IF EXISTS GetUserById $$
-- DELIMITER $$
-- CREATE PROCEDURE GetUserById(
--     IN p_user_id BIGINT,
--     OUT p_user_login VARCHAR(255),
--     OUT p_user_role VARCHAR(255)
-- )
-- BEGIN
--     -- Declare variables to store user information
--     DECLARE v_user_login VARCHAR(255);
--     DECLARE v_user_role VARCHAR(255);

--     -- Retrieve user information based on the user ID
--     SELECT u.login, r.name AS role
--     INTO
--         v_user_login,
--         v_user_role
--     FROM
--         User u
--     INNER JOIN
--         Role r ON u.role_id = r.id
--     WHERE
--         u.id = p_user_id;
--     -- Set the output parameters
--     SET p_user_login = v_user_login;
--     SET p_user_role = v_user_role;
-- END $$
-- DELIMITER ;

-- CALL GetUserById(1, @userLogin, @userRole);
-- select @userLogin as login, @userRole as `role`;
