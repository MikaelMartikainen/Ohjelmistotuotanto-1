using System;
using Microsoft.Maui.Controls;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Ohjelmistotuotanto_1
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseHelper dbHelper;
        
        public MainPage()
        {
            try
            {
                InitializeComponent();
                
                // Try to get DatabaseHelper from DI
                try
                {
                    var handler = Application.Current?.Handler;
                    var context = handler?.MauiContext;
                    
                    if (context != null)
                    {
                        var services = context.Services;
                        dbHelper = services.GetService<DatabaseHelper>();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DI error: {ex.Message}");
                }
                
                // Fallback to direct instantiation if DI fails
                if (dbHelper == null)
                {
                    dbHelper = new DatabaseHelper();
                    System.Diagnostics.Debug.WriteLine("Using fallback DatabaseHelper");
                }
                
                // Initialize the database connection
                InitializeDatabaseSafe();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainPage initialization error: {ex.Message}");
                DisplayAlert("Error", $"Initialization error: {ex.Message}", "OK").ConfigureAwait(false);
            }
        }

        private async void InitializeDatabaseSafe()
        {
            try
            {
                await InitializeDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Safe initialization error: {ex.Message}");
                await DisplayAlert("Database Error", $"Could not connect to the database: {ex.Message}", "OK");
            }
        }

        private async Task InitializeDatabase()
        {
            try
            {
                bool isConnected = await dbHelper.TestConnectionAsync();
                if (isConnected)
                {
                    // Only load data if connection successful
                    await LoadData();
                }
                else
                {
                    await DisplayAlert("Connection Error", "Unable to connect to the database. Please check your connection settings.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                await DisplayAlert("Error", $"Database initialization error: {ex.Message}", "OK");
            }
        }

        private async Task LoadData()
        {
            try
            {
                // Try to select from alue table
                DataTable dt = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`alue`");
                if (dt.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} areas:");
                    foreach (DataRow row in dt.Rows)
                    {
                        // In the new schema, there's just alue_id and nimi
                        System.Diagnostics.Debug.WriteLine($"Alue: {row["alue_id"]} - {row["nimi"]}");
                    }
                    
                    // Try to get some mokki data as well
                    DataTable mokkiDt = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`mokki` LIMIT 5");
                    if (mokkiDt.Rows.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"\nFound {mokkiDt.Rows.Count} cottages:");
                        foreach (DataRow row in mokkiDt.Rows)
                        {
                            System.Diagnostics.Debug.WriteLine($"Mökki: {row["mokki_id"]} - {row["mokkinimi"]}");
                        }
                    }
                    
                    await DisplayAlert("Success", "Database connection successful and data loaded!", "OK");
                }
                else
                {
                    // If alue table is empty, insert some data
                    await dbHelper.ExecuteNonQueryAsync("INSERT INTO `vn`.`alue` (`nimi`) VALUES ('Pohjois-Suomi'), ('Itä-Suomi'), ('Etelä-Suomi'), ('Länsi-Suomi')");
                    await DisplayAlert("Info", "No data found in the alue table. Sample data has been added.", "OK");
                    
                    // Reload data
                    await LoadData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load data error: {ex.Message}");
                
                if (ex.Message.Contains("doesn't exist"))
                {
                    await DisplayAlert("Database Error", "The database schema doesn't exist. Please run the database setup script.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                }
            }
        }

        private async void OnAlueidenHallintaClicked(object sender, EventArgs e)
        {
            try
            {
                // Use Navigation.PushAsync instead of Shell navigation
                await Navigation.PushAsync(new Aluehallintav2());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not navigate to the page.", "OK");
            }
        }

        private async void OnPalveluidenHallintaClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new PalveluidenHallinta());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not navigate to the page.", "OK");
            }
        }

        private async void OnMokkivarauksetClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new MokkiHallinta());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not navigate to the page.", "OK");
            }
        }

        private async void OnAsiakashallintaClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AsiakasHallinta());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not navigate to the page.", "OK");
            }
        }

        private async void OnLaskujenHallintaClicked(object sender, EventArgs e)
        {
            // Not implemented yet
        }

        private async void OnMajoittumisetAlueittainClicked(object sender, EventArgs e)
        {
            // Not implemented yet
        }

        private async void OnMajoittumistenRaportointiClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Varaushallinta());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Navigation Error", "Could not navigate to the page.", "OK");
            }
        }

        private async void OnOstetutPalvelutClicked(object sender, EventArgs e)
        {
            // Not implemented yet
        }
    }
}