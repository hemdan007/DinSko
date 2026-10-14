using DinSko.Models;
using Microsoft.Data.SqlClient;
using Isopoh.Cryptography.Argon2;
namespace DinSko.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<User> GetAll()
        {
            List<User> users = new List<User>(); // an empty list to collect all users from the database
            // create a connection to the database using the connectionString, The using statement automatically closes the connection after use.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // opens the connection
                string sql = "SELECT UserId, Name, Email, Role, PasswordHash " + "FROM [User]"; // SQL query to select all users from the User table
                using (SqlCommand command = new SqlCommand(sql, connection)) // SQL command to execute the query
                {
                    using (SqlDataReader reader = command.ExecuteReader()) // SQL data reader to read the data from the database
                    {
                        while (reader.Read()) // while loop reads each row of the result set
                        {
                            User user = new User() // create a new User object for the current row.
                            {
                                UserId = reader.GetInt32(0), // 0 means the first column in the result set, which is UserId
                                Name = reader.GetString(1), // 1 means the second column in the result set, which is Name
                                Email = reader.GetString(2), // 2 means the third column in the result set, which is Email
                                Role = (UserRole)reader.GetInt32(3), // 3 means the fourth column in the result set, which is Role - convert the int value from the DB to the UserRole enum
                                PasswordHash = reader.GetString(4) // 4 means the fifth column in the result set, which is PasswordHash
                            };
                            users.Add(user);  // Add the User to the list.
                        }
                    }
                }
            }
            return users;
        }

        public User GetById(int id)
        {
            User user = null; // initialize user to null
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT UserId, Name, Email, Role, PasswordHash " +
                "FROM [User] WHERE UserId = @UserId"; // SQL query to select a user by id from the User table
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id); // add parameter to the SQL command
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If there is a row in the result set
                        {
                            user = new User() // create a new User object for the current row
                            {
                                UserId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = (UserRole)reader.GetInt32(3),
                                PasswordHash = reader.GetString(4)
                            };
                        }
                    }
                }
            }
            return user;
        }

        public User GetByEmail(string email)
        {
            User user = null; // initialize user to null

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT UserId, Name, Email, Role, PasswordHash " +
                             "FROM [User] WHERE Email = @Email"; // SQL query to find a user by email

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email); // add email as a parameter

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User()
                            {
                                UserId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = (UserRole)reader.GetInt32(3),
                                PasswordHash = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return user;
        }


        public void Add(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO [User] (Name, Email, Role, PasswordHash) " +
                "VALUES (@Name, @Email, @Role, @PasswordHash)"; // SQL query to insert/create a new user
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    string passwordHash = Argon2.Hash(user.Password); // hash the password before storing it in the database
                    command.Parameters.AddWithValue("@Name", user.Name); // add the Name as a parameter.
                    command.Parameters.AddWithValue("@Email", user.Email); // add the Email as a parameter.
                    command.Parameters.AddWithValue("@Role", user.Role); // add the Role as a parameter.
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash); // add the hashed Password as a parameter.
                    command.ExecuteNonQuery(); // execute the SQL command and create the user in the database
                }
            }
        }

        public void Update(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE [User] " +
                "SET Name = @Name, Email = @Email, Role = @Role, PasswordHash = @PasswordHash " +
                "WHERE UserId = @UserId"; // SQL query to update a user
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    string passwordHash = Argon2.Hash(user.Password); // hash the password before storing it in the database
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.ExecuteNonQuery(); // execute the SQL command and update the user in the database
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM [User] " + "WHERE UserId = @UserId"; // SQL query to delete a user by its ID
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    command.ExecuteNonQuery(); // execute the SQL command and delete the user from the database
                }
            }
        }
        public void ChangePassword(int userId, string passwordHash)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE [User] " + "SET PasswordHash = @PasswordHash " +
                             "WHERE UserId = @UserId"; // SQL query to update the user's password hash

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    command.ExecuteNonQuery(); // execute the SQL command and update the password hash
                }
            }
        }
    }
}
