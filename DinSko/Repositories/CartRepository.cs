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
                            CartItem cartItem = new CartItem(); // create a new CartItem object for the current row.
                            cartItem.CartItemId = reader.GetInt32(0); // 0 means the first column, CartItemId.
                            cartItem.CartId = reader.GetInt32(1); // 1 means the second column, CartId.
                            cartItem.ProductVariantId = reader.GetInt32(2); // 2 means the third column, ProductVariantId.
                            cartItem.Quantity = reader.GetInt32(3); // 3 means the fourth column, Quantity.
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
    }
}