using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Ohjelmistotuotanto_1
{
    public partial class Laskuhallinta : ContentPage
    {
        // Direct database access
        private DatabaseHelper dbHelper;
        
        // Services for database operations
        private LaskuService _laskuService;
        
        // Current selected invoice
        private Lasku _selectedLasku;
        
        // List of all invoices
        private List<Lasku> _laskut;
        
        // DataTable for direct database access
        private DataTable laskuData;
        private int laskuID = -1;

        public Laskuhallinta()
        {
            InitializeComponent();
            
            // Initialize database helper and service
            dbHelper = new DatabaseHelper();
            _laskuService = new LaskuService();
            
            // Load invoices when page appears
            this.Appearing += Laskuhallinta_Appearing;
        }
        
        // Event handler for page appearing
        private async void Laskuhallinta_Appearing(object sender, EventArgs e)
        {
            // Show loading indicator or disable UI during loading
            try
            {
                // Load invoices from database - using both the service and direct access
                await LoadLaskuData();
                
                // Reset selection
                LaskuCollectionView.SelectedItem = null;
                UpdateUIState(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Laskujen lataaminen epäonnistui: {ex.Message}", "OK");
            }
        }
        
        // Load invoice data directly from database
        private async Task LoadLaskuData()
        {
            try
            {
                // Query to get invoice data with related information
                string query = @"
                    SELECT l.lasku_id, l.varaus_id, l.summa, l.alv, l.maksettu,
                           a.etunimi, a.sukunimi, 
                           m.mokkinimi,
                           v.varattu_alkupvm, v.varattu_loppupvm
                    FROM lasku l
                    JOIN varaus v ON l.varaus_id = v.varaus_id
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                    JOIN mokki m ON v.mokki_id = m.mokki_id
                    ORDER BY l.lasku_id DESC";
                
                // Get data directly
                laskuData = await dbHelper.GetDataAsync(query);
                
                // Convert to Lasku objects for the view
                _laskut = ConvertDataTableToLaskuList(laskuData);
                
                // Set the item source
                LaskuCollectionView.ItemsSource = _laskut;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading invoice data: {ex.Message}");
                throw;
            }
        }
        
        // Convert DataTable to List<Lasku>
        private List<Lasku> ConvertDataTableToLaskuList(DataTable dt)
        {
            List<Lasku> laskut = new List<Lasku>();
            
            foreach (DataRow row in dt.Rows)
            {
                Lasku lasku = new Lasku
                {
                    LaskuId = Convert.ToInt32(row["lasku_id"]),
                    VarausId = Convert.ToInt32(row["varaus_id"]),
                    Summa = Convert.ToDouble(row["summa"]),
                    Alv = Convert.ToDouble(row["alv"]),
                    Maksettu = Convert.ToBoolean(row["maksettu"]),
                    AsiakkaanNimi = $"{row["etunimi"]} {row["sukunimi"]}",
                    MokinNimi = row["mokkinimi"].ToString(),
                    AlkuPvm = Convert.ToDateTime(row["varattu_alkupvm"]),
                    LoppuPvm = Convert.ToDateTime(row["varattu_loppupvm"]),
                };
                laskut.Add(lasku);
            }
            
            return laskut;
        }

        // Selection handler for invoice list
        private void LaskuCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tarkistetaan, onko listalta valittu joku lasku
            if (LaskuCollectionView.SelectedItem != null)
            {
                // Get the selected invoice
                _selectedLasku = (Lasku)LaskuCollectionView.SelectedItem;
                laskuID = _selectedLasku.LaskuId;
                
                // Update UI with selected invoice details
                UpdateInvoiceDetails(_selectedLasku);
                
                // Enable action buttons
                UpdateUIState(true);
                
                // Disable "Mark as paid" button if invoice is already paid
                MerkitseMaksetuksiButton.IsEnabled = !_selectedLasku.Maksettu;
            }
            else
            {
                // No invoice selected
                _selectedLasku = null;
                laskuID = -1;
                
                // Hide details and disable buttons
                UpdateUIState(false);
            }
        }
        
        // Update the invoice details panel
        private void UpdateInvoiceDetails(Lasku lasku)
        {
            if (lasku != null)
            {
                DetailLaskuId.Text = lasku.LaskuId.ToString();
                DetailVarausId.Text = lasku.VarausId.ToString();
                DetailAsiakas.Text = lasku.AsiakkaanNimi;
                DetailMokki.Text = lasku.MokinNimi;
                DetailAjankohta.Text = $"{lasku.AlkuPvm:dd.MM.yyyy} - {lasku.LoppuPvm:dd.MM.yyyy}";
                DetailSumma.Text = $"{lasku.Summa:C} (ALV {lasku.Alv:C})";
                DetailTila.Text = lasku.Maksettu ? "Maksettu" : "Maksamaton";
                
                // Show details panel
                LaskuDetailsBorder.IsVisible = true;
            }
            else
            {
                // Hide details panel
                LaskuDetailsBorder.IsVisible = false;
            }
        }
        
        // Update UI state based on selection
        private void UpdateUIState(bool hasSelection)
        {
            PoistaLaskuButton.IsEnabled = hasSelection;
            MerkitseMaksetuksiButton.IsEnabled = hasSelection && _selectedLasku != null && !_selectedLasku.Maksettu;
        }

        // Luo PDF -painikkeen tapahtumankäsittelijä
        private async void LuoPdfButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            try
            {
                // Selvitetään, kumpi laskutustapa on valittu
                bool isEmailFormat = TulostusSahkopostilaskuRadio.IsChecked;
                string laskutustapa = isEmailFormat ? "Sähköpostilasku" : "Paperilasku";
                
                // Generate the PDF (or text file in this demo)
                string filePath = await PdfGenerator.GenerateInvoicePdf(_selectedLasku, isEmailFormat);
                
                // If email format, send by email
                if (isEmailFormat)
                {
                    bool emailSent = await PdfGenerator.SendInvoiceByEmail(filePath, _selectedLasku);
                    if (emailSent)
                    {
                        await DisplayAlert("Sähköpostilasku", 
                            $"Lasku on lähetetty sähköpostitse asiakkaalle {_selectedLasku.AsiakkaanNimi}. " +
                            $"Tiedosto tallennettu: {filePath}", "OK");
                    }
                }
                else
                {
                    // Just show that the file was created
                    await DisplayAlert("PDF Luotu", 
                        $"PDF on luotu muodossa: {laskutustapa}. Tiedosto tallennettu: {filePath}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"PDF-tiedoston luominen epäonnistui: {ex.Message}", "OK");
            }
        }

        // Laskun lisääminen -tapahtumankäsittelijä
        private async void LisaaLaskuButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Direct database access for reservations without invoices
                string query = @"
                    SELECT v.varaus_id, a.etunimi, a.sukunimi, m.mokkinimi, 
                           v.varattu_alkupvm, v.varattu_loppupvm,
                           m.hinta AS mokki_hinta,
                           DATEDIFF(v.varattu_loppupvm, v.varattu_alkupvm) AS nights
                    FROM varaus v
                    JOIN asiakas a ON v.asiakas_id = a.asiakas_id
                    JOIN mokki m ON v.mokki_id = m.mokki_id
                    LEFT JOIN lasku l ON v.varaus_id = l.varaus_id
                    WHERE l.lasku_id IS NULL
                    ORDER BY v.varattu_alkupvm DESC";
                
                DataTable reservations = await dbHelper.GetDataAsync(query);
                
                if (reservations.Rows.Count == 0)
                {
                    await DisplayAlert("Tietoa", "Ei löytynyt varauksia, joille voisi luoda laskun.", "OK");
                    return;
                }
                
                // Build a list of reservation options
                List<string> options = new List<string>();
                foreach (DataRow row in reservations.Rows)
                {
                    string reservationInfo = $"Varaus #{row["varaus_id"]} - {row["etunimi"]} {row["sukunimi"]} - " +
                                            $"{row["mokkinimi"]} - " +
                                            $"{Convert.ToDateTime(row["varattu_alkupvm"]):dd.MM.yyyy} - " +
                                            $"{Convert.ToDateTime(row["varattu_loppupvm"]):dd.MM.yyyy}";
                    options.Add(reservationInfo);
                }
                
                // Let user select a reservation
                string selected = await DisplayActionSheet("Valitse varaus laskulle", "Peruuta", null, options.ToArray());
                
                if (selected == "Peruuta" || string.IsNullOrEmpty(selected))
                    return;
                
                // Find the selected reservation data
                int index = options.IndexOf(selected);
                DataRow selectedRow = reservations.Rows[index];
                
                int varausId = Convert.ToInt32(selectedRow["varaus_id"]);
                double mokkiHinta = Convert.ToDouble(selectedRow["mokki_hinta"]);
                int nights = Convert.ToInt32(selectedRow["nights"]);
                
                // Calculate invoice total
                double totalAmount = mokkiHinta * nights;
                double alv = totalAmount * 0.24; // 24% VAT
                
                // Create invoice directly using the database
                // Get next available invoice ID
                string getMaxIdQuery = "SELECT COALESCE(MAX(lasku_id), 0) + 1 FROM lasku";
                object maxIdResult = await dbHelper.ExecuteScalarAsync(getMaxIdQuery);
                int newLaskuId = Convert.ToInt32(maxIdResult);
                
                // Insert new invoice
                string insertQuery = "INSERT INTO lasku (lasku_id, varaus_id, summa, alv, maksettu) VALUES (@laskuId, @varausId, @summa, @alv, @maksettu)";
                
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", newLaskuId },
                    { "@varausId", varausId },
                    { "@summa", totalAmount },
                    { "@alv", alv },
                    { "@maksettu", false }
                };
                
                await dbHelper.ExecuteNonQueryAsync(insertQuery, parameters);
                
                // Reload invoices data
                await LoadLaskuData();
                
                // Find and select the new invoice
                Lasku newInvoice = _laskut.FirstOrDefault(l => l.LaskuId == newLaskuId);
                if (newInvoice != null)
                {
                    LaskuCollectionView.SelectedItem = newInvoice;
                }
                
                await DisplayAlert("Lasku Lisätty", $"Uusi lasku #{newLaskuId} on lisätty varaukselle #{varausId}.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Laskun lisääminen epäonnistui: {ex.Message}", "OK");
            }
        }

        // Mark invoice as paid
        private async void MerkitseMaksetuksiButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null || laskuID == -1)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            try
            {
                // Confirm from the user
                bool confirm = await DisplayAlert("Vahvista", 
                    $"Haluatko merkitä laskun #{_selectedLasku.LaskuId} maksetuksi?", 
                    "Kyllä", "Ei");
                
                if (!confirm)
                    return;
                
                // Mark as paid directly in database
                string query = "UPDATE lasku SET maksettu = 1 WHERE lasku_id = @laskuId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", laskuID }
                };
                
                await dbHelper.ExecuteNonQueryAsync(query, parameters);
                
                // Update UI
                _selectedLasku.Maksettu = true;
                DetailTila.Text = "Maksettu";
                MerkitseMaksetuksiButton.IsEnabled = false;
                
                // Refresh data
                await LoadLaskuData();
                
                // Find and select the updated invoice
                Lasku updatedInvoice = _laskut.FirstOrDefault(l => l.LaskuId == laskuID);
                if (updatedInvoice != null)
                {
                    LaskuCollectionView.SelectedItem = updatedInvoice;
                }
                
                await DisplayAlert("Merkitty Maksetuksi", $"Lasku #{laskuID} on merkitty maksetuksi.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe merkittäessä laskua maksetuksi: {ex.Message}", "OK");
            }
        }
        
        // Laskun poistaminen -tapahtumankäsittelijä
        private async void PoistaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null || laskuID == -1)
            {
                await DisplayAlert("Virhe", "Valitse poistettava lasku.", "OK");
                return;
            }

            try
            {
                // Ask for confirmation
                bool confirm = await DisplayAlert("Vahvista poisto", 
                    $"Haluatko varmasti poistaa laskun #{laskuID}?", 
                    "Poista", "Peruuta");
                
                if (!confirm)
                    return;
                
                // Delete directly from database
                string query = "DELETE FROM lasku WHERE lasku_id = @laskuId";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@laskuId", laskuID }
                };
                
                await dbHelper.ExecuteNonQueryAsync(query, parameters);
                
                // Reload data
                await LoadLaskuData();
                
                // Reset selection and UI
                LaskuCollectionView.SelectedItem = null;
                _selectedLasku = null;
                laskuID = -1;
                
                // Hide details and disable buttons
                UpdateUIState(false);
                LaskuDetailsBorder.IsVisible = false;
                
                await DisplayAlert("Lasku Poistettu", "Valittu lasku on poistettu.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Laskun poistaminen epäonnistui: {ex.Message}", "OK");
            }
        }

        // Tulosta Lasku -tapahtumankäsittelijä
        private async void TulostaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            try
            {
                // Selvitetään valittu tulostustapa
                bool isEmailFormat = TulostusSahkopostilaskuRadio.IsChecked;
                string tulostustapa = isEmailFormat ? "Sähköpostilasku" : "Paperilasku";
                
                // Generate the PDF (or text file in this demo)
                string filePath = await PdfGenerator.GenerateInvoicePdf(_selectedLasku, isEmailFormat);
                
                // If email format, send by email
                if (isEmailFormat)
                {
                    bool emailSent = await PdfGenerator.SendInvoiceByEmail(filePath, _selectedLasku);
                    if (emailSent)
                    {
                        await DisplayAlert("Sähköpostilasku", 
                            $"Lasku on lähetetty sähköpostitse asiakkaalle {_selectedLasku.AsiakkaanNimi}.", "OK");
                    }
                }
                else
                {
                    // Simulate printing
                    await Task.Delay(1000); // Simulate printing operation
                    await DisplayAlert("Tulostus", $"Lasku #{_selectedLasku.LaskuId} tulostettu muodossa: {tulostustapa}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Tulostaminen epäonnistui: {ex.Message}", "OK");
            }
        }
    }
}
