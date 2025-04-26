using System;
using System.Threading.Tasks;

namespace Ohjelmistotuotanto_1
{
    class TestDatabaseConnection
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Testing database connection...");
            DatabaseHelper db = new DatabaseHelper();
            try
            {
                bool success = await db.TestConnectionAsync();
                if (success)
                {
                    Console.WriteLine("Connection successful!");
                }
                else
                {
                    Console.WriteLine("Connection failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
} 