using System;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace Ohjelmistotuotanto_1
{
    class CheckDatabase
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Checking database connection and schema...");
            
            // Simple connection string with minimal parameters
            string connectionString = "Server=127.0.0.1;Port=3307;Uid=root;Pwd=root;SslMode=None;";
            
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    Console.WriteLine("Connected to MySQL server successfully!");
                    
                    // Check if vn database exists
                    string sql = "SHOW DATABASES LIKE 'vn'";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            Console.WriteLine("Database 'vn' exists!");
                            
                            // Check tables
                            conn.ChangeDatabase("vn");
                            sql = "SHOW TABLES";
                            using (MySqlCommand tablesCmd = new MySqlCommand(sql, conn))
                            {
                                using (MySqlDataReader reader = (MySqlDataReader)await tablesCmd.ExecuteReaderAsync())
                                {
                                    if (reader.HasRows)
                                    {
                                        Console.WriteLine("Tables in vn database:");
                                        while (await reader.ReadAsync())
                                        {
                                            Console.WriteLine($"- {reader.GetString(0)}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No tables found in vn database!");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Database 'vn' does not exist!");
                            Console.WriteLine("Would you like to create the database and tables? (y/n)");
                            string answer = Console.ReadLine();
                            if (answer.ToLower() == "y")
                            {
                                Console.WriteLine("Creating database and tables...");
                                // Create database
                                sql = "CREATE DATABASE IF NOT EXISTS vn DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin";
                                using (MySqlCommand createDbCmd = new MySqlCommand(sql, conn))
                                {
                                    await createDbCmd.ExecuteNonQueryAsync();
                                    Console.WriteLine("Database 'vn' created successfully!");
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Number}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
} 