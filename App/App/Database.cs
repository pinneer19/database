using ConsoleTables;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace App
{
    internal class Database
    {

        MySqlConnection connection = new("server=127.0.0.1;port=3306;username=root;password=;database=online_store");

        public void OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                    Console.WriteLine("\tDatabase connection opened");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Connection closed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        private void PrintTableForQuery(string query)
        {
            MySqlDataAdapter adapter = new(query, connection);
            MySqlCommandBuilder builder = new(adapter);

            DataTable dataTable = new();

            adapter.Fill(dataTable);

            ConsoleTable consoleTable = new(dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray());
            foreach (DataRow row in dataTable.Rows)
            {
                consoleTable.AddRow(row.ItemArray);
            }

            consoleTable.Write();
        }

        public bool GetUser(string username, string password, out string? login, out string? role, out int? userId)
        {
            MySqlCommand command = new("SELECT id, role_id FROM User WHERE login = @username AND password = @password", connection);

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    userId = reader.GetInt32("id");
                    reader.Close();

                    var getRoleCommand = new MySqlCommand("GetUserById", connection);
                    getRoleCommand.CommandType = CommandType.StoredProcedure;
                    getRoleCommand.Parameters.AddWithValue("p_user_id", userId);

                    getRoleCommand.Parameters.Add(new MySqlParameter("p_user_login", MySqlDbType.VarChar, 255));
                    getRoleCommand.Parameters["p_user_login"].Direction = ParameterDirection.Output;

                    getRoleCommand.Parameters.Add(new MySqlParameter("p_user_role", MySqlDbType.VarChar, 255));
                    getRoleCommand.Parameters["p_user_role"].Direction = ParameterDirection.Output;

                    getRoleCommand.ExecuteNonQuery();

                    login = getRoleCommand.Parameters["p_user_login"].Value.ToString() ?? throw new Exception("User not found");
                    role = getRoleCommand.Parameters["p_user_role"].Value.ToString() ?? throw new Exception("User not found");

                    return true;
                }
            }

            login = null; role = null; userId = null;
            return false;
        }

        public void GetAdminUsers()
        {
            string query = @"
                SELECT u.id, login, password, r.name as role FROM `user` u
                JOIN role r ON u.role_id = r.id
                WHERE role_id IN (SELECT id FROM `role` WHERE name IN ('seller', 'customer'))
            ";

            PrintTableForQuery(query);
        }

        public void GetAdminCustomers()
        {
            string query = "SELECT * FROM customer";

            PrintTableForQuery(query);
        }

        public void GetUserLogs(int user_id)
        {
            string query = "GetUserLogsById";

            MySqlDataAdapter adapter = new(query, connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("p_user_id", user_id);

            MySqlCommandBuilder builder = new(adapter);

            DataTable dataTable = new();

            adapter.Fill(dataTable);

            ConsoleTable consoleTable = new(dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray());
            foreach (DataRow row in dataTable.Rows)
            {
                consoleTable.AddRow(row.ItemArray);
            }

            consoleTable.Write();
        }

        public void GetCustomerCart(int user_id)
        {
            string query = "GetCustomerCart";

            MySqlDataAdapter adapter = new(query, connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("p_user_id", user_id);
            adapter.SelectCommand.Parameters.Add(new MySqlParameter("product_quantity", MySqlDbType.Int32));
            adapter.SelectCommand.Parameters["product_quantity"].Direction = ParameterDirection.Output;


            MySqlCommandBuilder builder = new(adapter);

            DataTable dataTable = new();

            adapter.Fill(dataTable);

            ConsoleTable consoleTable = new(dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray());
            consoleTable.Options.EnableCount = false;
            foreach (DataRow row in dataTable.Rows)
            {
                consoleTable.AddRow(row.ItemArray);
            }

            consoleTable.Write();

            Console.WriteLine("\n\tProduct quantity: " + adapter.SelectCommand.Parameters["product_quantity"].Value.ToString());
        }

        public void GetProducts()
        {
            string query = "SELECT id, name, description FROM product";

            PrintTableForQuery(query);
        }

        public void AddProductToCart(int user_id, int product_id, int total_amount)
        {

            string query = "AddProductToCart";

            var getRoleCommand = new MySqlCommand(query, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            getRoleCommand.Parameters.AddWithValue("p_user_id", user_id);
            getRoleCommand.Parameters.AddWithValue("p_product_id", product_id);
            getRoleCommand.Parameters.AddWithValue("p_total_amount", total_amount);


            getRoleCommand.ExecuteNonQuery();

            Console.WriteLine("Product was added successfully!");

            GetCustomerCart(user_id);
        }

        public void RemoveProductFromCart(int user_id, int product_id)
        {
            string query = "RemoveProductFromCart";

            var getRoleCommand = new MySqlCommand(query, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            getRoleCommand.Parameters.AddWithValue("p_user_id", user_id);
            getRoleCommand.Parameters.AddWithValue("p_product_id", product_id);

            getRoleCommand.ExecuteNonQuery();

            Console.WriteLine("Product was removed successfully!");

            GetCustomerCart(user_id);
        }

        public void MakeOrder(int user_id)
        {
            string query = "SELECT id FROM customercart WHERE customer_id = (SELECT id FROM customer WHERE user_id = @p_user_id)";

            MySqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@p_user_id", user_id);

            object result = command.ExecuteScalar();
            int cart_id = Convert.ToInt32(result);

            // Retrieve all unique product IDs in the customer cart
            string productIdsQuery = "SELECT product_id FROM CustomerCart_Product WHERE customer_cart_id = @customer_cart_id";

            MySqlCommand productIdsCommand = new(productIdsQuery, connection);
            productIdsCommand.Parameters.AddWithValue("@customer_cart_id", cart_id);

            MySqlDataReader productIdsReader = productIdsCommand.ExecuteReader();
            List<int> productIds = new();

            while (productIdsReader.Read())
            {
                int productId = productIdsReader.GetInt32("product_id");

                productIds.Add(productId);

            }

            string sellerProductsQuery = @"
                    SELECT sp.id, p.name, p.description, sp.seller_id, sp.price
                    FROM Seller_Product sp
                    INNER JOIN CustomerCart_Product ccp ON sp.product_id = ccp.product_id
                    INNER JOIN Product p ON sp.product_id = p.id
                    WHERE ccp.customer_cart_id = @customer_cart_id
                    AND sp.product_id = @product_id";

            productIdsReader.Close();

            query = "INSERT INTO `Order` (customer_id, date, total_amount) VALUES ((SELECT id FROM customer WHERE user_id = @p_user_id), CURDATE(), 0)";
            var orderCmd = new MySqlCommand(query, connection);
            orderCmd.Parameters.AddWithValue("@p_user_id", user_id);
            orderCmd.ExecuteNonQuery();


            query = "SELECT max(id) from `Order`";
            orderCmd = new MySqlCommand(query, connection);

            result = orderCmd.ExecuteScalar();
            int order_id = Convert.ToInt32(result);


            foreach (int productId in productIds)
            {
                MySqlDataAdapter adapter = new(sellerProductsQuery, connection);

                adapter.SelectCommand.Parameters.AddWithValue("@customer_cart_id", cart_id);
                adapter.SelectCommand.Parameters.AddWithValue("@product_id", productId);

                MySqlCommandBuilder builder = new(adapter);

                DataTable dataTable = new();

                adapter.Fill(dataTable);

                ConsoleTable consoleTable = new(dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray());

                consoleTable.Options.EnableCount = false;

                foreach (DataRow row in dataTable.Rows)
                {
                    consoleTable.AddRow(row.ItemArray);
                }

                consoleTable.Write();
                Console.Write("Enter id of product to pick: ");
                bool isParsed = int.TryParse(Console.ReadLine(), out int sp_id);
                while (!isParsed)
                {
                    Console.WriteLine("incorrect input! Try again");
                    Console.Write("Enter id of product to pick: ");
                    isParsed = int.TryParse(Console.ReadLine(), out sp_id); ;
                }

                query = "SELECT id FROM customercart_product WHERE customer_cart_id = @cart_id AND product_id=@product_id";
                var cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@cart_id", cart_id);
                cmd.Parameters.AddWithValue("@product_id", productId);

                var ccp_id = Convert.ToInt32(cmd.ExecuteScalar());

                query = "CreateOrderItem";

                var createOrderItemCmd = new MySqlCommand(query, connection);
                createOrderItemCmd.CommandType = CommandType.StoredProcedure;

                createOrderItemCmd.Parameters.AddWithValue("p_order_id", order_id);
                createOrderItemCmd.Parameters.AddWithValue("p_seller_product_id", sp_id);
                createOrderItemCmd.Parameters.AddWithValue("p_customercart_product_id", ccp_id);
                createOrderItemCmd.Parameters.Add(new MySqlParameter("p_orderitem_id", MySqlDbType.Int32));
                createOrderItemCmd.Parameters["p_orderitem_id"].Direction = ParameterDirection.Output;

                createOrderItemCmd.ExecuteNonQuery();


            }

            ClearCustomerCart(user_id);

        }

        public int GetUserIdWithRole(Role role, int user_id)
        {
            string tableName = "";
            switch (role)
            {
                case Role.ADMIN:
                    {
                        tableName = "admin";
                        break;
                    }
                case Role.SELLER:
                    {
                        tableName = "seller";
                        break;
                    }
                case Role.CUSTOMER:
                    {
                        tableName = "customer";
                        break;
                    }
            }

            var query = $"SELECT id FROM {tableName} WHERE user_id = @p_user_id";
            var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@p_user_id", user_id);

            return Convert.ToInt32(cmd.ExecuteScalar());

        }

        public void ClearCustomerCart(int user_id)
        {
            var customer_id = GetUserIdWithRole(Role.CUSTOMER, user_id);
            var query = "ClearCustomerCart";

            var clearCmd = new MySqlCommand(query, connection);
            clearCmd.CommandType = CommandType.StoredProcedure;

            clearCmd.Parameters.AddWithValue("p_customer_id", customer_id);
            clearCmd.Parameters["p_customer_id"].Direction = ParameterDirection.Input;

            clearCmd.ExecuteNonQuery();

        }

        public void GetSellerProducts(int user_id) {
            var seller_id = GetUserIdWithRole(Role.SELLER, user_id);
            
            var query = $"SELECT sp.product_id as 'product id for sale', p.name, p.description, sp.price FROM seller_product sp JOIN product p ON sp.product_id = p.id WHERE seller_id={seller_id}";
            
            PrintTableForQuery(query);
        }

        public void AddSellerProduct(int product_id, int user_id, float price)
        {
            var seller_id = GetUserIdWithRole(Role.SELLER, user_id);

            var query = $"AddSellerItem";
            var getRoleCommand = new MySqlCommand(query, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            getRoleCommand.Parameters.AddWithValue("p_seller_id", seller_id);
            getRoleCommand.Parameters.AddWithValue("p_product_id", product_id);
            getRoleCommand.Parameters.AddWithValue("p_price", price);

            getRoleCommand.ExecuteNonQuery();

            Console.WriteLine("Product was added successfully!");

            GetSellerProducts(user_id);
        }

        public void GetSellerOrders(int user_id)
        {
            var seller_id = GetUserIdWithRole(Role.SELLER, user_id);

            var query = 
                @$"SELECT oi.order_id, oi.total_amount, oi.seller_product_id, p.name FROM orderitem oi
                JOIN seller_product sp ON oi.seller_product_id = sp.id
                JOIN product p ON sp.product_id = p.id
                WHERE sp.seller_id = {seller_id}";

            PrintTableForQuery(query);
        }

        public void RemoveSellerProduct(int user_id, int sp_id)
        {
            var seller_id = GetUserIdWithRole(Role.SELLER, user_id);

            var query = $"DELETE FROM seller_product WHERE seller_id = {seller_id} AND product_id = {sp_id}";

            MySqlCommand cmd = new(query, connection);
            cmd.ExecuteNonQuery();

            Console.WriteLine("Product removed successfully!");

            GetSellerProducts(user_id);
        }

        public void CreateUser(int role_id, string login, string password, out int user_id)
        {

            var query = $"INSERT INTO User (role_id, login, password) VALUES ({role_id}, '{login}', '{password}')"; 

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();

            query = "SELECT MAX(id) FROM User";
            object result = (new MySqlCommand(query, connection)).ExecuteScalar();

            user_id = Convert.ToInt32(result);
        }

        public void CreateCustomer(int user_id, string first_name, string last_name, string email, string phone, string address)
        {
            var query = @$"
                INSERT INTO customer (user_id, first_name, last_name, email, phone, address)
                VALUES ({user_id}, '{first_name}', '{last_name}', '{email}', '{phone}', '{address}')
            ";

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public void CreateSeller (int user_id, string name)
        {
            var query = $"INSERT INTO seller (user_id, name) VALUES ({user_id}, '{name}')";

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public void CreateAdmin(int user_id)
        {
            var query = $"INSERT INTO admin (user_id) VALUES ({user_id})";

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public void AddCustomerCard(int user_id, string card)
        {
            int customer_id = GetUserIdWithRole(Role.CUSTOMER, user_id);

            var query = $"INSERT INTO card (customer_id, card_number) VALUES ({user_id}, '{card}')";

            var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int user_id)
        {
            var query = $"DELETE FROM USER WHERE id = {user_id}";
            (new MySqlCommand(query, connection)).ExecuteNonQuery();
        }

        public void UpdateUser(int user_id)
        {
            var query = $"SELECT r.name FROM role r WHERE r.id = (SELECT u.role_id FROM user u WHERE u.id = {user_id})";

            var cmd = new MySqlCommand(query, connection);
            string? role = Convert.ToString(cmd.ExecuteScalar());
            if(role == null)
            {
                Console.WriteLine("Check user id!");
                return;
            }

            switch(role)
            {
                case "seller":
                    {
                        query = $"SELECT * FROM seller WHERE user_id = {user_id}";
                        PrintTableForQuery(query);

                        Console.Write("Enter new name for seller: ");
                        string name = Console.ReadLine();

                        query = $"UPDATE seller SET name = '{name}' WHERE user_id = {user_id}";

                        new MySqlCommand(query, connection).ExecuteNonQuery();
                        break;
                    }
                case "customer":
                    {

                        query = $"SELECT * FROM customer WHERE user_id = {user_id}";
                        PrintTableForQuery(query);

                        Console.Write("Enter field name for update (first_name, last_name, email, phone, address): ");
                        string field = Console.ReadLine();

                        Console.Write("Enter field value: ");
                        string value = Console.ReadLine();

                        query = $"UPDATE customer SET {field} = '{value}' WHERE user_id = {user_id}";

                        new MySqlCommand(query, connection).ExecuteNonQuery();

                        break;
                    }
            }
        }
    }
}
