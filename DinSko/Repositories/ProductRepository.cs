using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Product> GetAll()
        {
            List<Product> products = new List<Product>(); // an empty list to colect all products from the database
            using (SqlConnection connection = new SqlConnection(_connectionString)) // Create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            {
                connection.Open(); // opens the connection
                string sql = "SELECT ProductId, Name, Description, Price FROM Product"; // SQL query to select all products from the Product table
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        while (reader.Read()) // while loop reads each row of the result set
                        {
                            Product product = new Product();
                            product.ProductId = reader.GetInt32(0); // 0 means the first column in the result set, which is ProductId
                            product.Name = reader.GetString(1); // 1 means the second column in the result set, which is Name
                            product.Description = reader.IsDBNull(2) ? null : reader.GetString(2); // 2 means the third column in the result set, which is Description
                            product.Price = reader.GetDecimal(3); // 3 means the fourth column in the result set, which is Price
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public Product GetById(int id)
        {
            Product product = null; // initialize product to null
            using (SqlConnection connection = new SqlConnection(_connectionString)) // Create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            {
                connection.Open(); // opens the connection
                string sql = "SELECT ProductId, Name, Description, Price FROM Product WHERE ProductId = @ProductId"; // SQL query to select a product by id from the Product table
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    command.Parameters.AddWithValue("@ProductId", id); // add parameter to the SQL command
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        if (reader.Read()) // if there is a row in the result set
                        {
                            product = new Product();
                            product.ProductId = reader.GetInt32(0); // 0 means the first column in the result set, which is ProductId
                            product.Name = reader.GetString(1); // 1 means the second column in the result set, which is Name
                            product.Description = reader.IsDBNull(2) ? null : reader.GetString(2); // If Description in DB is NULL, set it to null - otherwise, read it as a string.
                            product.Price = reader.GetDecimal(3); // 3 means the fourth column in the result set, which is Price
                        }
                    }
                }
            }
            return product;
        }

        public void Add(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString)) // Create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            {
                connection.Open(); // opens the connection
                string sql = "INSERT INTO Product (Name, Description, Price) " + "VALUES (@Name, @Description, @Price)";
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    command.Parameters.AddWithValue("@Name", product.Name); // Add the product name as a parameter.
                    command.Parameters.AddWithValue("@Description", product.Description); // Add the product description as a parameter.
                    command.Parameters.AddWithValue("@Price", product.Price); // Add the product price as a parameter.
                    command.ExecuteNonQuery(); // Execute the SQL command and add the product to the database.
                }
            }
        }

        public void Update(Product product)
        {
        }

        public void Delete(int id)
        {
        }
    }
}