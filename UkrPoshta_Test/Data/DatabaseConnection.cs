using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UkrPoshta_Test.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // This is just for testing purposes and should be removed in production
        public void TestDatabaseConnection()
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful!");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"An error occurred while opening the connection: {ex.Message}");
                }
            }
        }
    }
}
