using App;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Data;
using ConsoleTables;
using System.Runtime.CompilerServices;


namespace App
{
    enum Role
    {
        ADMIN, CUSTOMER, SELLER
    }

    class Program
    {
        private static Database Database = new();
        private static bool IsAuthenticated { get; set; } = false;
        private static Role? UserRole { get; set; } = null;
        private static int UserId { get; set; }

        static void UpdateRole(string roleName) {
            UserRole = roleName switch
            {
                "admin" => (Role?)Role.ADMIN,
                "customer" => (Role?)Role.CUSTOMER,
                "seller" => (Role?)Role.SELLER,
                _ => null
            };
        }
 
        static Tuple<string?, string?> AuthenticateUser(string username, string password)
        {
            IsAuthenticated = Database.GetUser(username, password, out string? login, out string? role, out int? user_id);

            if (user_id != null) UserId = user_id.Value;

            return new Tuple<string?, string?>(login, role);
        }

        static void GetCommandList()
        {
            Console.WriteLine("\t\tAvailable commands:");
            Console.WriteLine("- exit: Exits the application.");
            Console.WriteLine("- clear: Clears the console screen.");
            Console.WriteLine("- login: Prompts for login and password to authenticate the user.");
            Console.WriteLine("- help: Displays available commands and their usage.");
            Console.WriteLine("- register: Registration process.");
            Console.WriteLine("- logout: Logout authenticated user.");
        }

        static void GetAdminCommands()
        {
            Console.WriteLine("\n\t\tAdmin commands:");
            Console.WriteLine("- users: Displays all users.");
            Console.WriteLine("- customers: Displays all customers.");
            Console.WriteLine("- logs: Displays selected user logs.");
            Console.WriteLine("- create: Creates user.");
            Console.WriteLine("- update: Updates selected user info.");
            Console.WriteLine("- delete: Deletes selected user.");
        }

        static void GetSellerCommands()
        {
            Console.WriteLine("\n\t\tSeller commands:");
            Console.WriteLine("- add: Add product for sale.");
            Console.WriteLine("- sale: Display all products listed for sale.");
            Console.WriteLine("- orders: Display your orders.");
            Console.WriteLine("- remove: Remove product for sale.");
            Console.WriteLine("- products: Display list of available products in store.");
        }

        static void GetCustomerCommands()
        {
            Console.WriteLine("\n\t\tCustomer commands:");
            Console.WriteLine("- cart: Displays current cart state.");
            Console.WriteLine("- products: Display list of available products in store.");
            Console.WriteLine("- add: Add product to cart using its id.");
            Console.WriteLine("- remove: Remove product from cart using its id.");
            Console.WriteLine("- order: Make order using current cart state.");
            Console.WriteLine("- card: Add pay card.");
        }

        static void ProcessCommand(string command)
        {
            switch (command)
            {
                case "exit":
                    {
                        Environment.Exit(0);
                        Database.CloseConnection();
                        break;
                    }
                case "clear":
                    {
                        Console.Clear();
                        break;
                    }
                case "login":
                    {
                        if(IsAuthenticated)
                        {
                            Console.WriteLine("Logout first for being login available!");
                            return;
                        }
                        Console.Write("Enter login: ");
                        string username = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string password = Console.ReadLine();

                        var loginTuple = AuthenticateUser(username!, password!);
                        if (!IsAuthenticated)
                        {
                            Console.WriteLine("Check your login input!");
                        }
                        else
                        {
                            UpdateRole(loginTuple.Item2!);
                            Console.WriteLine("Login successful!");
                        }

                        Thread.Sleep(1500);
                        Console.Clear();

                        break;
                    }
                case "help":
                    {
                        GetCommandList();
                        if(IsAuthenticated)
                        {
                            switch(UserRole)
                            {
                                case Role.ADMIN:
                                    GetAdminCommands();
                                    break;

                                case Role.CUSTOMER:
                                    GetCustomerCommands();
                                    break;

                                case Role.SELLER:
                                    GetSellerCommands();
                                    break;

                                case null: default: break;
                            }
                        }
                        break;
                    }
                case "logout":
                    {
                        if (IsAuthenticated)
                        {
                            IsAuthenticated = false;
                            Console.WriteLine("Logout was successful!");
                        }
                        else Console.WriteLine("Login first to logout!");

                        break;
                    }
                case "register":
                    {
                        if (IsAuthenticated)
                        {
                            Console.WriteLine("You are logged out now!");
                            IsAuthenticated = false;
                        }

                        Console.Write("Pick user role(seller, customer, admin): ");
                        var role = Console.ReadLine();

                        Console.Write("Pass user login and password by space: ");
                        var input = Console.ReadLine();
                        var splitInput = input?.Split(' ');

                        var login = splitInput?.Length > 0 ? splitInput[0] : null;
                        var password = splitInput?.Length > 1 ? splitInput[1] : null;

                        if(login == null || password == null)
                        {
                            Console.WriteLine("Incorrect input!");
                            return;
                        }

                        switch (role) {
                            case "seller":
                                {
                                    Database.CreateUser(2, login, password, out int user_id);

                                    Console.Write("Enter name: ");
                                    string name = Console.ReadLine();

                                    if (name.Length == 0) { Console.WriteLine("Name cannot be emtpy!"); return; }

                                    Database.CreateSeller(user_id, name);
                                    Console.WriteLine("Seller was created successfully!");
                                    break;
                                }
                            case "customer":
                                {
                                    Database.CreateUser(3, login, password, out int user_id);

                                    Console.Write("Enter first amd last name by space: ");
                                    var (first_n, last_n) = Console.ReadLine().Split(' ') switch { var a => (a[0], a[1]) };

                                    Console.Write("Enter email: ");
                                    string email = Console.ReadLine();

                                    Console.Write("Enter phone number: ");
                                    string phone = Console.ReadLine();

                                    Console.Write("Enter address: ");
                                    string address = Console.ReadLine();


                                    Database.CreateCustomer(user_id, first_n, last_n, email, phone, address);

                                    Console.WriteLine("Customer was created successfully!");
                                    break;
                                }
                            case "admin":
                                {
                                    Database.CreateUser(1, login, password, out int user_id);

                                    Database.CreateAdmin(user_id);
                                    Console.WriteLine("Admin was created successfully!");

                                    break;
                                }
                            default: Console.WriteLine("Incorrect role!"); break;
                        }
                        
                        break;
                    }
                default:
                    {
                        bool cmdStatus = false;
                        if (UserRole != null)
                        {
                            switch (UserRole)
                            {
                                case Role.ADMIN:
                                    ProcessAdminCommand(command, out cmdStatus);
                                    break;
                                
                                case Role.CUSTOMER:
                                    ProcessCustomerCommand(command, out cmdStatus);
                                    break;

                                case Role.SELLER:
                                    ProcessSellerCommand(command, out cmdStatus);
                                    break;
                            }
                        }
                        if (cmdStatus == false)
                        {
                            Console.WriteLine("Invalid command. Try again.");
                            Thread.Sleep(1000);
                            Console.Clear();
                        }
                        break;
                    }
            }
        }

        static void ProcessAdminCommand(string cmd, out bool status)
        {
            switch (cmd)
            {
                case "users":
                    {
                        Database.GetAdminUsers();
                        status = true;
                        break;
                    }
                case "customers":
                    {
                        Database.GetAdminCustomers();
                        status = true;
                        break;
                    }
                case "logs":
                    {
                        status = true;
                        Console.Write("Enter user id: ");
                        _ = int.TryParse(Console.ReadLine(), out int user_id);
                        Database.GetUserLogs(user_id);
                        
                        break;
                    }
                case "delete":
                    {
                        status = true;
                        Database.GetAdminUsers();
                        Console.Write("Enter user id: ");
                        int.TryParse(Console.ReadLine(), out int user_id);
                        Database.DeleteUser(user_id);
                        Console.WriteLine("User was deleted successfully!");
                        Database.GetAdminUsers();
                        break;
                    }
                case "update":
                    {
                        status = true;
                        Database.GetAdminUsers();
                        Console.Write("Enter user id: ");
                        int.TryParse(Console.ReadLine(), out int user_id);
                        
                        Database.UpdateUser(user_id);

                        break;
                    }

                default:
                    {
                        status = false;
                        break;
                    }
            }
        }

        static void ProcessCustomerCommand(string cmd, out bool status)
        {
            switch (cmd)
            {
                case "cart":
                    {
                        Database.GetCustomerCart(UserId);
                        status = true;
                        break;
                    }
                case "products":
                    {
                        Database.GetProducts();
                        Console.WriteLine("Type 'add' to add product to cart");
                        status = true;
                        break;
                    }
                case "add":
                    {
                        status = true;
                        Database.GetProducts();
                        Console.Write("Enter product id: ");
                        bool isNumber = int.TryParse(Console.ReadLine(), out int product_id);
                        if(!isNumber)
                        {
                            Console.WriteLine("Id must be number!");
                            return;
                        }
                        Console.Write("Enter total amount (1 is default): ");
                        int total_amount = int.TryParse(Console.ReadLine(), out total_amount) ? total_amount : 1;
                        if(total_amount <= 0) { 
                            Console.WriteLine("It's not possible to add such amount of product!");
                            return;
                        }
                        
                        Database.AddProductToCart(UserId, product_id, total_amount);
                        break;
                    }
                case "remove":
                    {
                        status = true;
                        Console.Write("Enter product id: ");
                        int.TryParse(Console.ReadLine(), out int product_id);
                        
                        Database.RemoveProductFromCart(UserId, product_id);
                        break;
                    }
                case "order":
                    {
                        status = true;
                        Database.MakeOrder(UserId);
                        Database.ClearCustomerCart(UserId);
                        Console.WriteLine("Order was created successfully!");
                        break;
                    }
                case "card":
                    {
                        status = true;
                        Console.Write("Enter card number: ");
                        var card = Console.ReadLine();
                        if (card.Length < 16)
                        {
                            Console.WriteLine("Check card number!");
                            return;
                        }

                        Database.AddCustomerCard(UserId, card);
                        break;
                    }
                default:
                    {
                        status = false;
                        break;
                    }
            }
        }

        static void ProcessSellerCommand(string cmd, out bool status)
        {
            switch (cmd)
            {
                case "add":
                    {
                        Database.GetProducts();
                        
                        Console.Write("Enter product id: ");
                        _ = int.TryParse(Console.ReadLine(), out int product_id);

                        Console.Write("Enter desired selling price: ");
                        _ = float.TryParse(Console.ReadLine().Replace('.', ','), out float price);

                        Database.AddSellerProduct(product_id, UserId, price);

                        status = true;
                        
                        break;
                    }
                case "products":
                    {
                        Database.GetProducts();
                        Console.WriteLine("Type 'add' to add product for sale");
                        status = true;
                        break;
                    }
                case "sale":
                    {
                        Database.GetSellerProducts(UserId);
                        Console.WriteLine("Type 'remove' to remove product from sale");
                        status = true;
                        break;
                    }
                case "orders":
                    {
                        status = true;

                        Database.GetSellerOrders(UserId);
                        break;
                    }
                case "remove":
                    {
                        status = true;

                        Database.GetSellerProducts(UserId);
                        Console.Write("Pick product id to remove: ");
                        _ = int.TryParse(Console.ReadLine(), out int sp_id);

                        Database.RemoveSellerProduct(UserId, sp_id);

                        break;
                    }
                default:
                    {
                        status = false;
                        break;
                    }
            }
        }

        static void Main(string[] args)
        {
            Database.OpenConnection();

            Console.WriteLine("\n\t\t\t\t\t\tOnline Store");

            while (true)
            {
                
                Console.WriteLine("Type help to Get command list!");
                string command = Console.ReadLine();

                ProcessCommand(command);

            }
        }
    }
}