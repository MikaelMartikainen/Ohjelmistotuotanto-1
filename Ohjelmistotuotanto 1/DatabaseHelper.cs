using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ohjelmistotuotanto_1
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=127.0.0.1;Port=3307;Database=vn;Uid=root;Pwd=root;AllowPublicKeyRetrieval=True;SslMode=None;";
        private int maxRetries = 3;

        // Test connection
        public async Task<bool> TestConnectionAsync()
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync().ConfigureAwait(false);
                
                // Perform a simple query to really test the connection
                using (MySqlCommand cmd = new MySqlCommand("SELECT 1", conn))
                {
                    await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                }
                
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"MySQL Connection Error: {ex.Message}, Error Code: {ex.Number}");
                // Specific MySQL error handling
                switch (ex.Number)
                {
                    case 1042: // Unable to connect to any of the specified MySQL hosts
                        System.Diagnostics.Debug.WriteLine("Cannot connect to MySQL server. Check if server is running.");
                        break;
                    case 1045: // Access denied (wrong username/password)
                        System.Diagnostics.Debug.WriteLine("Access denied. Check username and password.");
                        break;
                    case 1049: // Unknown database
                        System.Diagnostics.Debug.WriteLine("Unknown database. Check database name.");
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unhandled MySQL error: {ex.Message}");
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection Error: {ex.Message}");
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        // Get data with query
        public async Task<DataTable> GetDataAsync(string query)
        {
            DataTable dt = new DataTable();
            MySqlConnection conn = null;
            int retryCount = 0;
            
            while (retryCount < maxRetries)
            {
                try
                {
                    conn = new MySqlConnection(connectionString);
                    await conn.OpenAsync().ConfigureAwait(false);
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            dt.Load(reader);
                        }
                    }
                    
                    // If we got here, no need to retry
                    break;
                }
                catch (MySqlException ex)
                {
                    retryCount++;
                    System.Diagnostics.Debug.WriteLine($"Database error (attempt {retryCount}/{maxRetries}): {ex.Message}");
                    
                    // If it's our last retry, throw the exception
                    if (retryCount >= maxRetries)
                    {
                        throw;
                    }
                    
                    // Otherwise, wait before retrying
                    await Task.Delay(1000 * retryCount).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"General error: {ex.Message}");
                    throw; // Re-throw to allow the caller to handle the exception
                }
                finally
                {
                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            
            return dt;
        }

        // Get data with parameters
        public async Task<DataTable> GetDataWithParamsAsync(string query, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync().ConfigureAwait(false);
                
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return dt;
        }

        // Execute non-query (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync().ConfigureAwait(false);
                
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        // Get scalar value (COUNT, SUM, etc.)
        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync().ConfigureAwait(false);
                
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}