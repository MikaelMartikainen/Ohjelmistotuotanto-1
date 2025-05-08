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
        // Services for database operations
        private LaskuService _laskuService;
        
        // Current selected invoice
        private Lasku _selectedLasku;
        
        // List of all invoices
        private List<Lasku> _laskut;

        public Laskuhallinta()
        {
            InitializeComponent();
            
            // Initialize service
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
                // Load invoices from database
                _laskut = await _laskuService.GetLaskutAsync();
                LaskuCollectionView.ItemsSource = _laskut;
                
                // Reset selection
                LaskuCollectionView.SelectedItem = null;
                UpdateUIState(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Laskujen lataaminen epäonnistui: {ex.Message}", "OK");
            }
        }

        // Tähn koodia kun valitaan lasku listalta (xaml puolella LaskuCollectionView) 
        private void LaskuCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tarkistetaan, onko listalta valittu joku lasku
            if (LaskuCollectionView.SelectedItem != null)
            {
                // Get the selected invoice
                _selectedLasku = (Lasku)LaskuCollectionView.SelectedItem;
                
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
                // Get reservations without invoices
                DataTable reservations = await _laskuService.GetReservationsWithoutInvoicesAsync();
                
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
                
                // Create invoice
                int newInvoiceId = await _laskuService.CreateLaskuAsync(varausId, totalAmount, alv);
                
                // Reload invoices
                _laskut = await _laskuService.GetLaskutAsync();
                LaskuCollectionView.ItemsSource = _laskut;
                
                // Select the new invoice
                Lasku newInvoice = _laskut.FirstOrDefault(l => l.LaskuId == newInvoiceId);
                if (newInvoice != null)
                {
                    LaskuCollectionView.SelectedItem = newInvoice;
                }
                
                await DisplayAlert("Lasku Lisätty", $"Uusi lasku #{newInvoiceId} on lisätty varaukselle #{varausId}.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Laskun lisääminen epäonnistui: {ex.Message}", "OK");
            }
        }

        // Mark invoice as paid
        private async void MerkitseMaksetuksiButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null)
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
                
                // Mark as paid in database
                await _laskuService.MarkAsPaidAsync(_selectedLasku.LaskuId);
                
                // Update UI
                _selectedLasku.Maksettu = true;
                DetailTila.Text = "Maksettu";
                MerkitseMaksetuksiButton.IsEnabled = false;
                
                // Refresh collection view
                LaskuCollectionView.ItemsSource = null;
                LaskuCollectionView.ItemsSource = _laskut;
                
                await DisplayAlert("Merkitty Maksetuksi", $"Lasku #{_selectedLasku.LaskuId} on merkitty maksetuksi.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe merkittäessä laskua maksetuksi: {ex.Message}", "OK");
            }
        }
        
        // Laskun poistaminen -tapahtumankäsittelijä
        private async void PoistaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (_selectedLasku == null)
            {
                await DisplayAlert("Virhe", "Valitse poistettava lasku.", "OK");
                return;
            }

            try
            {
                // Ask for confirmation
                bool confirm = await DisplayAlert("Vahvista poisto", 
                    $"Haluatko varmasti poistaa laskun #{_selectedLasku.LaskuId}?", 
                    "Poista", "Peruuta");
                
                if (!confirm)
                    return;
                
                // Delete from database
                await _laskuService.DeleteLaskuAsync(_selectedLasku.LaskuId);
                
                // Remove from list
                _laskut.Remove(_selectedLasku);
                
                // Update UI
                LaskuCollectionView.ItemsSource = null;
                LaskuCollectionView.ItemsSource = _laskut;
                LaskuCollectionView.SelectedItem = null;
                
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
