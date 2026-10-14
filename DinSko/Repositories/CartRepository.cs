using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public CartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public Cart GetByUserId(int userId)
        {
            Cart cart = null; // initialize cart to null.
            // create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // open the database connection.
                string sql = "SELECT CartId, UserId " + "FROM Cart " +
                             "WHERE UserId = @UserId"; // SQL query to get the cart for a specific user.
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    command.Parameters.AddWithValue("@UserId", userId); // add the user ID as a parameter.
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        if (reader.Read()) // if there is a cart for the user.
                        {
                            cart = new Cart(); // create a new Cart object.
                            cart.CartId = reader.GetInt32(0); // 0 means the first column, CartId.
                            cart.UserId = reader.GetInt32(1); // 1 means the second column, UserId.
                        }
                    }
                }
            }
            return cart;
        }

        public void Create(Cart cart)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Cart (UserId) " + "VALUES (@UserId)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", cart.UserId);
                    command.ExecuteNonQuery(); // execute the SQL command and create the cart in the database.
                }
            }
        }

        public void Delete(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM  Cart WHERE CartId = @CartId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<CartItem> GetItems(int cartId)
        {
            List<CartItem> cartItems = new List<CartItem>(); // an empty list to collect the cart items.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT CartItemId, CartId, ProductVariantId, Quantity " + "FROM CartItem " +
                     "WHERE CartId = @CartId"; // SQL query to select all items for a specific cart.
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    using (SqlDataReader reader = command.ExecuteReader()) // execute the query and read the results.
                    {
                        while (reader.Read()) // read each cart item from the result set.
                        {
                            CartItem cartItem = new CartItem() // create a new CartItem object for the current row.
                            {
                                CartItemId = reader.GetInt32(0), // 0 means the first column, CartItemId.
                                CartId = reader.GetInt32(1), // 1 means the second column, CartId.
                                ProductVariantId = reader.GetInt32(2), // 2 means the third column, ProductVariantId.
                                Quantity = reader.GetInt32(3) // 3 means the fourth column, Quantity.
                            };
                            cartItems.Add(cartItem); // add the CartItem to the list.
                        }
                    }
                }
                return cartItems;
            }
        }

        public void AddItem(CartItem item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO CartItem (CartId, ProductVariantId, Quantity) " +
                             "VALUES (@CartId, @ProductVariantId, @Quantity)"; // SQL query to add a cart item.
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartId", item.CartId); // add the cart ID as a parameter.
                    command.Parameters.AddWithValue("@ProductVariantId", item.ProductVariantId); // add the product variant ID as a parameter.
                    command.Parameters.AddWithValue("@Quantity", item.Quantity); // add the quantity as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and add the cart item to the database.
                }
            }
        }

        public void UpdateItem(CartItem item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE CartItem " + "SET Quantity = @Quantity " +
                             "WHERE CartItemId = @CartItemId"; // SQL query to update the quantity of a cart item.
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartItemId", item.CartItemId); // add the cart item ID as a parameter.
                    command.Parameters.AddWithValue("@Quantity", item.Quantity); // add the updated quantity as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and update the cart item in the database.
                }
            }
        }

        public void DeleteItem(int itemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM CartItem WHERE CartItemId = @CartItemId"; // SQL query to delete a cart item by its ID.
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartItemId", itemId); // add the cart item ID as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and delete the cart item from the database.
                }
            }
        }

        public void ClearCart(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM CartItem WHERE CartId = @CartId"; // SQL query to delete all items (not the cart itself) from a specific cart.
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId); // add the cart ID as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and clear the cart.
                }
            }
        }
        public bool AddItemIfInStock(CartItem item)
        {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Start a transaction to check stock and add the item safely
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string stockSql =
                                "SELECT Stock FROM ProductVariant " +
                                "WHERE ProductVariantId = @ProductVariantId";

                            int stock;

                            using (SqlCommand command = new SqlCommand(stockSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ProductVariantId", item.ProductVariantId);

                                object result = command.ExecuteScalar();

                                // Check that the selected variant exists
                                if (result == null || result == DBNull.Value)
                                {
                                    transaction.Rollback();
                                    return false;
                                }

                                stock = Convert.ToInt32(result);
                            }

                            // Check that enough stock is available
                            if (item.Quantity <= 0 || item.Quantity > stock)
                            {
                                transaction.Rollback();
                                return false;
                            }

                            // Check whether this variant already exists in the cart
                            string existingSql =
                                "SELECT CartItemId, Quantity FROM CartItem " +
                                "WHERE CartId = @CartId AND ProductVariantId = @ProductVariantId";

                            int existingItemId = 0;
                            int existingQuantity = 0;

                            using (SqlCommand command = new SqlCommand(existingSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@CartId", item.CartId);
                                command.Parameters.AddWithValue("@ProductVariantId", item.ProductVariantId);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        existingItemId = reader.GetInt32(0);
                                        existingQuantity = reader.GetInt32(1);
                                    }
                                }
                            }

                            // Add to the existing quantity if the variant is already in the cart
                            if (existingItemId > 0)
                            {
                                int newQuantity = existingQuantity + item.Quantity;

                                if (newQuantity > stock)
                                {
                                    transaction.Rollback();
                                    return false;
                                }

                                string updateSql =
                                    "UPDATE CartItem SET Quantity = @Quantity " +
                                    "WHERE CartItemId = @CartItemId AND CartId = @CartId";

                                using (SqlCommand command = new SqlCommand(updateSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Quantity", newQuantity);
                                    command.Parameters.AddWithValue("@CartItemId", existingItemId);
                                    command.Parameters.AddWithValue("@CartId", item.CartId);
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Add a new item to the cart
                                string insertSql =
                                    "INSERT INTO CartItem (CartId, ProductVariantId, Quantity) " +
                                    "VALUES (@CartId, @ProductVariantId, @Quantity)";

                                using (SqlCommand command = new SqlCommand(insertSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@CartId", item.CartId);
                                    command.Parameters.AddWithValue("@ProductVariantId", item.ProductVariantId);
                                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
        }

        public bool UpdateItemQuantity(int cartId, int itemId, int quantity)
        {
            // Reject invalid quantities
            if (quantity < 1 || quantity > 10)
            {
                return false;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Update the item only if it belongs to this cart and stock is sufficient
                string sql =
                    "UPDATE CartItem " +
                    "SET Quantity = @Quantity " +
                    "WHERE CartItemId = @CartItemId " +
                    "AND CartId = @CartId " +
                    "AND @Quantity <= (" +
                    "SELECT Stock FROM ProductVariant " +
                    "WHERE ProductVariant.ProductVariantId = CartItem.ProductVariantId)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@CartItemId", itemId);
                    command.Parameters.AddWithValue("@CartId", cartId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool RemoveItemFromCart(int cartId, int itemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Delete only an item that belongs to the specified cart
                string sql =
                    "DELETE FROM CartItem " +
                    "WHERE CartItemId = @CartItemId AND CartId = @CartId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CartItemId", itemId);
                    command.Parameters.AddWithValue("@CartId", cartId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}