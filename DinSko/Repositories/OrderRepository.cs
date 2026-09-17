    using DinSko.Models;
    using Microsoft.Data.SqlClient;
    namespace DinSko.Repositories
    {
        public class OrderRepository : IOrderRepository
        {
            private readonly string _connectionString;

            public OrderRepository(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            public IEnumerable<Order> GetAll()
            {
                List<Order> orders = new List<Order>(); // an empty list to collect all orders from the database
                // create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open(); // opens the connection
                    string sql = "SELECT OrderId, UserId, OrderDate, Status " + "FROM [Order]";  // SQL query to select all orders from the Order table
                    using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                    {
                        using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                        {
                            while (reader.Read()) // while loop reads each row of the result set
                            {
                                Order order = new Order() // Create a new order object for the current row
                                {
                                    OrderId = reader.GetInt32(0), // 0 means the first column in the result set, which is OrderId
                                    UserId = reader.GetInt32(1), // 1 means the second column in the result set, which is UserId
                                    OrderDate = reader.GetDateTime(2), // 2 means the third column in the result set, which is OrderDate
                                    Status = (OrderStatus)reader.GetInt32(3) // 3 means the fourth column in the result set, which is Status
                                };
                                orders.Add(order); // Add the order to the list.
                            }
                        }
                    }
                }
                return orders;
            }

            public Order GetById(int id)
            {
                Order order = null; // initialize order to null
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT OrderId, UserId, OrderDate, Status " + " FROM [Order] WHERE OrderId = @OrderId"; // SQL query to select an order by id from the Order table
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", id); // add parameter to the SQL command
                        using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                        {
                            if (reader.Read()) // If there is a row in the result set
                            {
                                order = new Order() // Create a new order object for the current row  
                                {
                                    OrderId = reader.GetInt32(0), 
                                    UserId = reader.GetInt32(1), 
                                    OrderDate = reader.GetDateTime(2), 
                                    Status = (OrderStatus)reader.GetInt32(3) 
                                };
                            }
                        }
                    }
                }
                return order;
            }

            public IEnumerable<Order> GetByUserId(int userId)
            {
                List<Order> orders = new List<Order>(); // an empty list to collect all orders for a specific user from the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT OrderId, UserId, OrderDate, Status " + "FROM [Order]" + " WHERE UserId = @UserId"; // SQL query to select all orders for a specific user
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId); // add parameter to the SQL command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order()
                                {
                                    OrderId = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    OrderDate = reader.GetDateTime(2),
                                    Status = (OrderStatus)reader.GetInt32(3)
                                };
                                orders.Add(order);
                            }
                        }
                    }
                }
                return orders;
            }

            public void Create(Order order)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO [Order] (UserId, OrderDate, Status) " + 
                    "VALUES (@UserId, @OrderDate, @Status)"; // SQL query to insert/create a new order
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", order.UserId);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@Status", order.Status);
                        command.ExecuteNonQuery(); // execute the SQL command and create the order in the database
                    }
                }
            }

            public void UpdateStatus(int orderId, OrderStatus status)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE [Order] " + 
                    "SET Status = @Status " + "WHERE OrderId = @OrderId"; // SQL query to update the status of an order
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.ExecuteNonQuery(); // execute the SQL command and update the order status in the database
                    }
                }
            }

            public void Delete(int orderId)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM [Order] " + "WHERE OrderId = @OrderId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);
                        command.ExecuteNonQuery(); // execute the SQL command and delete the order from the database
                    }
                }
            }

            public IEnumerable<OrderItem> GetItems(int orderId)
            {
                List<OrderItem> orderItems = new List<OrderItem>(); // an empty list to collect all order items for a specific order from the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT OrderItemId, OrderId, ProductVariantId, Quantity, UnitPrice " +
                    "FROM OrderItem " + "WHERE OrderId = @OrderId"; // SQL query to select all order items for a specific order
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId); // add parameter to the SQL command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderItem orderItem = new OrderItem()
                                {
                                    OrderItemId = reader.GetInt32(0),
                                    OrderId = reader.GetInt32(1),
                                    ProductVariantId = reader.GetInt32(2),
                                    Quantity = reader.GetInt32(3),
                                    UnitPrice = reader.GetDecimal(4)
                                };
                                orderItems.Add(orderItem);
                            }
                        }
                    }
                }
                return orderItems;
            }

            public void AddItem(OrderItem item)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO OrderItem (OrderId, ProductVariantId, Quantity, UnitPrice) " +
                                 "VALUES (@OrderId, @ProductVariantId, @Quantity, @UnitPrice)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", item.OrderId);
                        command.Parameters.AddWithValue("@ProductVariantId", item.ProductVariantId);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        command.ExecuteNonQuery(); // execute the SQL command and add the order item to the database
                    }
                }
            }

            public void DeleteItem(int itemId)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM OrderItem " + "WHERE OrderItemId = @OrderItemId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderItemId", itemId);
                        command.ExecuteNonQuery(); // execute the SQL command and delete the order item from the database
                    }
                }
            }
        }
    }