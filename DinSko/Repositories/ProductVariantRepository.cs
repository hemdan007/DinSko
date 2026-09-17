using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly string _connectionString;

        public ProductVariantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IEnumerable<ProductVariant> GetAll()
        {
            List<ProductVariant> productVariants = new List<ProductVariant>(); // an empty list to collect all the product variants from the database
            // create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                connection.Open(); // opens the connection
                string sql = "SELECT ProductVariantId, ProductId, Size, Stock" + " FROM ProductVariant"; // SQL query to select all productVariants from the ProductVariant table
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        while (reader.Read()) // while loop reads each row of the result set
                        {
                            ProductVariant productVariant = new ProductVariant() // create a new ProductVariant object for the current row.
                            {
                                ProductVariantId = reader.GetInt32(0), // 0 means the first column in the result set, which is ProductVariantId
                                ProductId = reader.GetInt32(1), // 1 means the second column in the result set, which is ProductId
                                Size = reader.GetInt32(2), // 2 means the third column in the result set, which is Size
                                Stock = reader.GetInt32(3) // 3 means the fourth column in the result set, which is Stock
                            };
                            productVariants.Add(productVariant); // add the ProductVariant to the list.
                        }
                    }
                }
            }
            return productVariants;
        }

        public ProductVariant GetById(int id)
        {
            ProductVariant productVariant = null; // initialize productVariant to null.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); 

                string sql = "SELECT ProductVariantId, ProductId, Size, Stock " + "FROM ProductVariant " +
                             "WHERE ProductVariantId = @ProductVariantId"; // SQL query to select a productvariant by id from the ProductVariant table
                using (SqlCommand command = new SqlCommand(sql, connection))  
                {
                    command.Parameters.AddWithValue("@ProductVariantId", id); // add the product variant ID as a parameter.
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        if (reader.Read()) // If there is a row in the result set.
                        {
                            productVariant = new ProductVariant // create a new ProductVariant object for the row.
                            {
                                ProductVariantId = reader.GetInt32(0), 
                                ProductId = reader.GetInt32(1), 
                                Size = reader.GetInt32(2), 
                                Stock = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return productVariant;
        }

        public IEnumerable<ProductVariant> GetByProductId(int productId)
        {
            List<ProductVariant> productVariants = new List<ProductVariant>(); // an empty list to collect the product variants.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); 
                string sql = "SELECT ProductVariantId, ProductId, Size, Stock " + "FROM ProductVariant " +
                             "WHERE ProductId = @ProductId"; // SQL query to select all variants for a specific product.
                using (SqlCommand command = new SqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ProductId", productId); // add the product ID as a parameter.
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        while (reader.Read()) // read each product variant from the result set.
                        {
                            ProductVariant productVariant = new ProductVariant() // create a new ProductVariant object for the current row.
                            {
                                ProductVariantId = reader.GetInt32(0), 
                                ProductId = reader.GetInt32(1), 
                                Size = reader.GetInt32(2), 
                                Stock = reader.GetInt32(3)
                            };
                            productVariants.Add(productVariant);
                        }
                    }
                }
            }
            return productVariants;
        }

        public void Add(ProductVariant productVariant)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); 
                string sql = "INSERT INTO ProductVariant (ProductId, Size, Stock) " + "VALUES (@ProductId, @Size, @Stock)"; // SQL query to add a product variant.
                using (SqlCommand command = new SqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ProductId", productVariant.ProductId); // add the product ID as a parameter.
                    command.Parameters.AddWithValue("@Size", productVariant.Size); // add the product size as a parameter.
                    command.Parameters.AddWithValue("@Stock", productVariant.Stock); // add the product stock as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and add the product variant to the database.
                }
            }
        }

        public void Update(ProductVariant productVariant)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); 
                string sql = "UPDATE ProductVariant " + "SET ProductId = @ProductId, Size = @Size, Stock = @Stock " +
                             "WHERE ProductVariantId = @ProductVariantId"; // SQL query to update a product variant by its ID.
                using (SqlCommand command = new SqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ProductVariantId", productVariant.ProductVariantId); // add the product variant ID as a parameter.
                    command.Parameters.AddWithValue("@ProductId", productVariant.ProductId); // add the product ID as a parameter.
                    command.Parameters.AddWithValue("@Size", productVariant.Size); // add the updated size as a parameter.
                    command.Parameters.AddWithValue("@Stock", productVariant.Stock); // add the updated stock as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and update the product variant in the database.
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM ProductVariant" + " WHERE ProductVariantId = @ProductVariantId"; // SQL query to delete a product variant by its ID.
                using (SqlCommand command = new SqlCommand(sql, connection)) 
                {
                    command.Parameters.AddWithValue("@ProductVariantId", id); // add the product variant ID as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and delete the product variant from the database.
                }
            }
        }
    }
}