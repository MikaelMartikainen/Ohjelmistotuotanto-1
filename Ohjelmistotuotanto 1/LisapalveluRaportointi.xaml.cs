using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class LisapalveluRaportointi : ContentPage
    {
        // Database helper instance
        private DatabaseHelper _dbHelper;
        
        // Activity indicator
        private bool _isLoading;
        public bool IsLoading 
        { 
            get => _isLoading; 
            set 
            { 
                _isLoading = value; 
                OnPropertyChanged(nameof(IsLoading)); 
            } 
        }

        public LisapalveluRaportointi()
        {
            InitializeComponent();
            
            // Initialize database helper
            _dbHelper = new DatabaseHelper();
            
            // Set binding context
            BindingContext = this;
            
            // Set default date range - current month
            AloitusPvm.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            LopetusPvm.Date = DateTime.Now;
            
            // Initialize the report with empty list
            RaporttiCollectionView.ItemsSource = new List<string>();
        }

        private async void LuoRaporttiButton_Clicked(object sender, EventArgs e)
        {
            if (AluePicker.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse alue ennen raportin lataamista.", "OK");
                return;
            }

            // Tarkistetaan, että aikajakso on järkevä
            if (AloitusPvm.Date > LopetusPvm.Date)
            {
                await DisplayAlert("Virhe", "Aloituspäivämäärä ei voi olla loppupäivämäärän jälkeen.", "OK");
                return;
            }
            
            await LoadLisapalveluRaporttiAsync();
        }
        
        private async Task LoadLisapalveluRaporttiAsync()
        {
            try
            {
                IsLoading = true;
                
                // Valittu alue
                string valittuAlue = AluePicker.SelectedItem.ToString();
                
                // Format dates for SQL query
                string alkuPvm = AloitusPvm.Date.ToString("yyyy-MM-dd");
                string loppuPvm = LopetusPvm.Date.ToString("yyyy-MM-dd");
                
                // Query to get additional services data between the selected dates
                string query = @"
                    SELECT 
                        p.nimi AS palvelun_nimi,
                        COUNT(vp.varaus_id) AS kayttokerrat,
                        SUM(vp.lkm) AS kokonaismaara
                    FROM 
                        palvelu p
                    JOIN 
                        alue a ON p.alue_id = a.alue_id
                    LEFT JOIN 
                        varauksen_palvelut vp ON p.palvelu_id = vp.palvelu_id
                    LEFT JOIN 
                        varaus v ON vp.varaus_id = v.varaus_id AND v.varattu_alkupvm >= @alkuPvm AND v.varattu_loppupvm <= @loppuPvm
                    WHERE 
                        a.nimi = @alueNimi
                    GROUP BY 
                        p.palvelu_id
                    ORDER BY 
                        kokonaismaara DESC";
                
                // Prepare parameters
                var parameters = new Dictionary<string, object>
                {
                    { "@alueNimi", valittuAlue },
                    { "@alkuPvm", alkuPvm },
                    { "@loppuPvm", loppuPvm }
                };
                
                // Execute the query
                DataTable result = await _dbHelper.GetDataWithParamsAsync(query, parameters);
                
                // Create results list
                var tulokset = new List<string>
                {
                    $"Alue: {valittuAlue}",
                    $"Aikajakso: {AloitusPvm.Date:dd.MM.yyyy} - {LopetusPvm.Date:dd.MM.yyyy}",
                    $"Lisäpalveluiden käyttö: {result.Rows.Count} palvelua"
                };
                
                // If no results found, show a message
                if (result.Rows.Count == 0)
                {
                    tulokset.Add("Ei lisäpalveluita käytetty valitulla ajanjaksolla.");
                }
                else
                {
                    // Add report rows for each service with usage statistics
                    foreach (DataRow row in result.Rows)
                    {
                        string palveluNimi = row["palvelun_nimi"].ToString();
                        int kayttokerrat = row["kayttokerrat"] != DBNull.Value ? Convert.ToInt32(row["kayttokerrat"]) : 0;
                        int kokonaismaara = row["kokonaismaara"] != DBNull.Value ? Convert.ToInt32(row["kokonaismaara"]) : 0;
                        
                        if (kayttokerrat > 0)
                        {
                            tulokset.Add($"{palveluNimi}: {kayttokerrat} varausta, yhteensä {kokonaismaara} kpl");
                        }
                        else
                        {
                            tulokset.Add($"{palveluNimi}: Ei käyttöä valitulla ajanjaksolla");
                        }
                    }
                    
                    // Get total usage
                    string totalQuery = @"
                        SELECT 
                            COUNT(DISTINCT vp.varaus_id) AS varaukset_lisapalveluilla,
                            SUM(vp.lkm) AS lisapalveluiden_kokonaismaara
                        FROM 
                            varauksen_palvelut vp
                        JOIN 
                            palvelu p ON vp.palvelu_id = p.palvelu_id
                        JOIN 
                            alue a ON p.alue_id = a.alue_id
                        JOIN 
                            varaus v ON vp.varaus_id = v.varaus_id
                        WHERE 
                            a.nimi = @alueNimi
                            AND v.varattu_alkupvm >= @alkuPvm 
                            AND v.varattu_loppupvm <= @loppuPvm";
                    
                    DataTable totalResult = await _dbHelper.GetDataWithParamsAsync(totalQuery, parameters);
                    
                    if (totalResult.Rows.Count > 0 && totalResult.Rows[0]["varaukset_lisapalveluilla"] != DBNull.Value)
                    {
                        int varauksetLisapalveluilla = Convert.ToInt32(totalResult.Rows[0]["varaukset_lisapalveluilla"]);
                        int kokonaismaara = totalResult.Rows[0]["lisapalveluiden_kokonaismaara"] != DBNull.Value ? 
                            Convert.ToInt32(totalResult.Rows[0]["lisapalveluiden_kokonaismaara"]) : 0;
                        
                        tulokset.Add($"Yhteensä: {varauksetLisapalveluilla} varausta sisälsi lisäpalveluita");
                        tulokset.Add($"Lisäpalveluiden kokonaismäärä: {kokonaismaara} kpl");
                    }
                }
                
                // Update the UI with results
                RaporttiCollectionView.ItemsSource = tulokset;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Raportin lataus epäonnistui: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
