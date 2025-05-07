using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class MajoittumisenRaportointi : ContentPage
    {
        // Lista alueista Pickeriä varten
        public ObservableCollection<Alue> Alueet { get; set; }

        // Valittu alue
        public Alue ValittuAlue { get; set; }
        
        // Nykyinen raportti
        private ObservableCollection<RaporttiRivi> _nykyinenRaportti;
        
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

        public MajoittumisenRaportointi()
        {
            InitializeComponent();
            
            // Initialize database helper
            _dbHelper = new DatabaseHelper();
            
            // Tärkeää: aseta binding context tähän luokkaan
            BindingContext = this;

            // Asetetaan alustava aikajakso - kuukauden alusta tähän päivään
            AloitusPvm.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            LoppuPvm.Date = DateTime.Now;
                       
            // Luodaan tyhjä raportti näkyviin
            _nykyinenRaportti = new ObservableCollection<RaporttiRivi>();
            RaporttiCollectionView.ItemsSource = _nykyinenRaportti;
            
            // Load areas from database when page appears
            Loaded += MajoittumisenRaportointi_Loaded;
        }
        
        private async void MajoittumisenRaportointi_Loaded(object sender, EventArgs e)
        {
            await LoadAlueetFromDatabaseAsync();
        }
        
        private async Task LoadAlueetFromDatabaseAsync()
        {
            try
            {
                IsLoading = true;
                
                // Fetch areas from database
                string query = "SELECT alue_id, nimi FROM alue ORDER BY nimi";
                DataTable result = await _dbHelper.GetDataAsync(query);
                
                // Create ObservableCollection
                Alueet = new ObservableCollection<Alue>();
                
                // Add areas from database to collection
                foreach (DataRow row in result.Rows)
                {
                    Alueet.Add(new Alue 
                    { 
                        AlueId = Convert.ToInt32(row["alue_id"]), 
                        AlueNimi = row["nimi"].ToString() 
                    });
                }
                
                // Set as ItemsSource for the picker
                AluePicker.ItemsSource = Alueet;
                AluePicker.ItemDisplayBinding = new Binding("AlueNimi");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Alueiden lataus epäonnistui: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Lataa Raportti -napin käsittelijä
        private async void LataaRaporttiButton_Clicked(object sender, EventArgs e)
        {
            if (AluePicker.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse alue ennen raportin lataamista.", "OK");
                return;
            }

            // Tarkistetaan, että aikajakso on järkevä
            if (AloitusPvm.Date > LoppuPvm.Date)
            {
                await DisplayAlert("Virhe", "Aloituspäivämäärä ei voi olla loppupäivämäärän jälkeen.", "OK");
                return;
            }

            await LoadMajoitusRaporttiAsync();
        }
        
        private async Task LoadMajoitusRaporttiAsync()
        {
            try
            {
                IsLoading = true;
                
                // Valittu alue
                Alue valittuAlue = (Alue)AluePicker.SelectedItem;
                
                // Format dates for SQL query
                string alkuPvm = AloitusPvm.Date.ToString("yyyy-MM-dd");
                string loppuPvm = LoppuPvm.Date.ToString("yyyy-MM-dd");
                
                // Query to get accommodation data between the selected dates
                string query = @"
                    SELECT 
                        m.mokkinimi, 
                        COUNT(v.varaus_id) AS varausten_maara,
                        SUM(DATEDIFF(v.varattu_loppupvm, v.varattu_alkupvm)) AS yopymismaara
                    FROM 
                        varaus v
                    JOIN 
                        mokki m ON v.mokki_id = m.mokki_id
                    JOIN 
                        alue a ON m.alue_id = a.alue_id
                    WHERE 
                        a.alue_id = @alueId
                        AND v.varattu_alkupvm >= @alkuPvm 
                        AND v.varattu_loppupvm <= @loppuPvm
                    GROUP BY 
                        m.mokki_id
                    ORDER BY 
                        yopymismaara DESC";
                
                // Prepare parameters
                var parameters = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "@alueId", valittuAlue.AlueId },
                    { "@alkuPvm", alkuPvm },
                    { "@loppuPvm", loppuPvm }
                };
                
                // Execute the query
                DataTable result = await _dbHelper.GetDataWithParamsAsync(query, parameters);
                
                // Clear previous results
                _nykyinenRaportti.Clear();
                
                // Add header information
                _nykyinenRaportti.Add(new RaporttiRivi { 
                    RaporttiTiedot = $"Alue: {valittuAlue.AlueNimi}" 
                });
                
                _nykyinenRaportti.Add(new RaporttiRivi { 
                    RaporttiTiedot = $"Aikajakso: {AloitusPvm.Date:dd.MM.yyyy} - {LoppuPvm.Date:dd.MM.yyyy}" 
                });
                
                _nykyinenRaportti.Add(new RaporttiRivi { 
                    RaporttiTiedot = $"Yhteensä majoittumisia: {result.Rows.Count}" 
                });
                
                // If no results found, show a message
                if (result.Rows.Count == 0)
                {
                    _nykyinenRaportti.Add(new RaporttiRivi { 
                        RaporttiTiedot = "Ei majoittumisia valitulla ajanjaksolla."
                    });
                }
                else
                {
                    // Add report rows for each cottage with reservations
                    foreach (DataRow row in result.Rows)
                    {
                        string mokkiNimi = row["mokkinimi"].ToString();
                        int varaustenMaara = Convert.ToInt32(row["varausten_maara"]);
                        int yopymismaara = Convert.ToInt32(row["yopymismaara"]);
                        
                        _nykyinenRaportti.Add(new RaporttiRivi { 
                            RaporttiTiedot = $"{mokkiNimi}: {varaustenMaara} varausta, {yopymismaara} yötä" 
                        });
                    }
                    
                    // Get total nights
                    string totalQuery = @"
                        SELECT 
                            SUM(DATEDIFF(v.varattu_loppupvm, v.varattu_alkupvm)) AS total_nights
                        FROM 
                            varaus v
                        JOIN 
                            mokki m ON v.mokki_id = m.mokki_id
                        WHERE 
                            m.alue_id = @alueId
                            AND v.varattu_alkupvm >= @alkuPvm 
                            AND v.varattu_loppupvm <= @loppuPvm";
                    
                    object totalResult = await _dbHelper.ExecuteScalarAsync(totalQuery, parameters);
                    
                    if (totalResult != null && totalResult != DBNull.Value)
                    {
                        int totalNights = Convert.ToInt32(totalResult);
                        _nykyinenRaportti.Add(new RaporttiRivi { 
                            RaporttiTiedot = $"Yöpymisiä yhteensä: {totalNights}" 
                        });
                    }
                }
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

    // Luokka alueille
    public class Alue
    {
        public int AlueId { get; set; }
        public string AlueNimi { get; set; }
    }

    // Luokka raportin riveille
    public class RaporttiRivi
    {
        public string RaporttiTiedot { get; set; }
    }
}
