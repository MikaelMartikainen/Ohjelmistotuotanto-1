using System.Data;

namespace Ohjelmistotuotanto_1;


public partial class Varaushallinta : ContentPage
{
    private DatabaseHelper dbHelper;
    private DataTable VarausData;
    private DataTable AsiakasData;
    private DataTable MokkiData;
    private DataTable PalveluData;
    private int VarausID = -1;

    // Strings to store input values
    string varattuPvm;
    string vahvistusPvm;
    string varattuAlkuPvm;
    string varattuLoppuPvm;
    string asiakasId;
    string mokkiId;

    public Varaushallinta()
	{
        InitializeComponent();
        dbHelper = new DatabaseHelper();
        LoadVarausData();
    }

    private async void LoadVarausData()
    {
        try
        {
            // ladataan tiedot
            VarausData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`varaus` ORDER BY varaus_id");
            VarausCollectionView.ItemsSource = VarausData.DefaultView;

            AsiakasData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`asiakas` ORDER BY asiakas_id");
            AsiakasCollectionView.ItemsSource = AsiakasData.DefaultView;

            MokkiData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`mokki` ORDER BY mokki_id");
            MokkiCollectionView.ItemsSource = MokkiData.DefaultView;

           // PalveluData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`palvelu` ORDER BY palvelu_id");
          //  PalveluCollectionView.ItemsSource = PalveluData.DefaultView;


        }

        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Tietojen lataus epäonnistui: {ex.Message}", "OK");
        }
    }




    private void VarausCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;    //kun varaus valitaan sen tiedot tulevat näkyviin
            if (selectedItem != null)
            {
                VarausID = Convert.ToInt32(selectedItem.Row["varaus_id"]);

                VarausEntry.Text = selectedItem.Row["varattu_pvm"].ToString(); 
                VahvistuspvmEntry.Text = selectedItem.Row["vahvistus_pvm"].ToString(); 
                VarausAlkupvmEntry.Text = selectedItem.Row["varattu_alkupvm"].ToString(); 
                VarausLoppupvmEntry.Text = selectedItem.Row["varattu_loppupvm"].ToString(); 
                AsiakasIDEntry.Text = selectedItem.Row["asiakas_id"].ToString(); 
                MokkiIDEntry.Text = selectedItem.Row["mokki_id"].ToString(); 
            }
        }
    }


    private async void LisaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // Get values from entry fields
        varattuPvm = VarausEntry.Text;
        vahvistusPvm = VahvistuspvmEntry.Text;
        varattuAlkuPvm = VarausAlkupvmEntry.Text;
        varattuLoppuPvm = VarausLoppupvmEntry.Text;
        asiakasId = AsiakasIDEntry.Text;
        mokkiId = MokkiIDEntry.Text;
        
        // Validate that all fields are filled
        if (string.IsNullOrWhiteSpace(varattuPvm) ||
            string.IsNullOrWhiteSpace(vahvistusPvm) ||
            string.IsNullOrWhiteSpace(varattuAlkuPvm) ||
            string.IsNullOrWhiteSpace(varattuLoppuPvm) ||
            string.IsNullOrWhiteSpace(asiakasId) ||
            string.IsNullOrWhiteSpace(mokkiId))
        {
            await DisplayAlert("Huomio", "Kaikki kentät täytyy täyttää.", "OK");
            return;
        }

        try
        {
            // Create parameter dictionary for database query
            var tiedot = new Dictionary<string, object>
            {
                { "@varattu_pvm", varattuPvm },
                { "@vahvistus_pvm", vahvistusPvm },
                { "@varattu_alkupvm", varattuAlkuPvm },
                { "@varattu_loppupvm", varattuLoppuPvm },
                { "@asiakas_id", asiakasId },
                { "@mokki_id", mokkiId }
            };

            // Execute SQL query to insert new reservation
            int affectedRows = await dbHelper.ExecuteNonQueryAsync(
                "INSERT INTO `vn`.`varaus` (`varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`, `asiakas_id`, `mokki_id`) " +
                "VALUES (@varattu_pvm, @vahvistus_pvm, @varattu_alkupvm, @varattu_loppupvm, @asiakas_id, @mokki_id)",
                tiedot);

            if (affectedRows > 0)
            {
                // Reload data and show success message
                LoadVarausData();
                await DisplayAlert("Onnistui", "Varaus lisätty onnistuneesti", "OK");
            }
            else
            {
                await DisplayAlert("Virhe", "Varauksen lisääminen epäonnistui", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Varaus epäonnistui: {ex.Message}", "OK");
        }
    }


    private async void MuokkaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        if (VarausID == -1) 
        {
            await DisplayAlert("Huomio", "Valitse ensin muokattava varaus", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(VarausEntry.Text) ||
        string.IsNullOrWhiteSpace(VahvistuspvmEntry.Text) ||
        string.IsNullOrWhiteSpace(VarausAlkupvmEntry.Text) ||
        string.IsNullOrWhiteSpace(VarausLoppupvmEntry.Text) ||
        string.IsNullOrWhiteSpace(AsiakasIDEntry.Text) ||
        string.IsNullOrWhiteSpace(MokkiIDEntry.Text))
        {
            await DisplayAlert("Huomio", "Täytä kaikki kentät ennen tallentamista", "OK");
            return;
        }

        try
        {
            // Create parameter dictionary for database query
            var tiedot = new Dictionary<string, object>
        {
            { "@varattu_pvm", VarausEntry.Text },
            { "@vahvistus_pvm", VahvistuspvmEntry.Text },
            { "@varattu_alkupvm", VarausAlkupvmEntry.Text },
            { "@varattu_loppupvm", VarausLoppupvmEntry.Text },
            { "@asiakas_id", AsiakasIDEntry.Text },
            { "@mokki_id", MokkiIDEntry.Text },
            { "@varaus_id", VarausID } // The ID of the reservation being edited
        };



            int affectedRows = await dbHelper.ExecuteNonQueryAsync(
            "UPDATE `vn`.`varaus` " +
            "SET varattu_pvm = @varattu_pvm, " +
            "vahvistus_pvm = @vahvistus_pvm, " +
            "varattu_alkupvm = @varattu_alkupvm, " +
            "varattu_loppupvm = @varattu_loppupvm, " +
            "asiakas_id = @asiakas_id, " +
            "mokki_id = @mokki_id " +
            "WHERE varaus_id = @varaus_id", tiedot);

            if (affectedRows > 0)
            {
                // Show success message and reload data
                await DisplayAlert("Onnistui", "Varaus päivitetty onnistuneesti", "OK");
                LoadVarausData();
            }
            else
            {
                await DisplayAlert("Virhe", "Varauksen päivitys epäonnistui", "OK");
            }
        }
        catch (Exception ex)
        {
            // Show error message if something goes wrong
            await DisplayAlert("Virhe", $"Päivitys epäonnistui: {ex.Message}", "OK");
        }
    }

    private async void PoistaVarausNappi_Clicked(object sender, EventArgs e)
    {
        if (VarausID == -1)
        {
            await DisplayAlert("Huomio", "Valitse ensin poistettava varaus", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Varmista", $"Haluatko varmasti poistaa varauksen {VarausID}?", "Kyllä", "Ei");
        if (!confirm) return;

        try
        {
            // Create parameter dictionary for database query
            var tiedot = new Dictionary<string, object>
            {
                { "@varaus_id", VarausID }
            };

            // Execute SQL query to delete reservation
            int affectedRows = await dbHelper.ExecuteNonQueryAsync(
                "DELETE FROM `vn`.`varaus` WHERE varaus_id = @varaus_id", 
                tiedot);

            if (affectedRows > 0)
            {
                // Show success message and clear fields
                await DisplayAlert("Onnistui", "Varaus poistettu onnistuneesti", "OK");
                
                // Clear entry fields
                VarausEntry.Text = string.Empty;
                VahvistuspvmEntry.Text = string.Empty;
                VarausAlkupvmEntry.Text = string.Empty;
                VarausLoppupvmEntry.Text = string.Empty;
                AsiakasIDEntry.Text = string.Empty;
                MokkiIDEntry.Text = string.Empty;
                VarausID = -1;

                // Reload data
                LoadVarausData();
            }
            else
            {
                await DisplayAlert("Virhe", "Varauksen poistaminen epäonnistui", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Poistaminen epäonnistui: {ex.Message}", "OK");
        }
    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
            //kun asiakas valitaan, ID tuleee näkyvin
        if (e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)
            {
                AsiakasIDEntry.Text = selectedItem.Row["asiakas_id"].ToString();
            }
        }
    }

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
        if (e.CurrentSelection.Count > 0)
        {
            
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)
            {
                MokkiIDEntry.Text = selectedItem.Row["mokki_id"].ToString();
            }
        }
    }

    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = hakuvalitsija.SelectedItem as string;
        
        if (!string.IsNullOrEmpty(selectedItem))
        {
            VarausHaku_SearchButtonPressed(null, null);
        }
    }

    private async void VarausHaku_SearchButtonPressed(object sender, EventArgs e)
    {
        var haku = new Dictionary<string, string>
        {
            { "Varaus päivä", "varattu_pvm" },
            { "Vahvistuspäivämäärä", "vahvistus_pvm" },
            { "Varauksen alku", "varattu_alkupvm" },
            { "Varauksen loppu", "varattu_loppupvm" },
            { "Asiakas ID", "asiakas_id" },
            { "Mökki ID", "mokki_id" }
        };

        string searchText = VarausHaku.Text;
        string selectedCategory = hakuvalitsija.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(searchText) || string.IsNullOrWhiteSpace(selectedCategory))
        {
            await DisplayAlert("Virhe", "Valitse hakukategoria ja kirjoita hakusana.", "OK");
            return;
        }


        if (!haku.TryGetValue(selectedCategory, out string columnName))
        {
            await DisplayAlert("Virhe", "Tuntematon hakukategoria.", "OK");
            return;
        }


        try
        {
            DataTable result = await dbHelper.GetDataAsync($"SELECT * FROM `vn`.`varaus` WHERE {columnName} LIKE '{searchText}%'");

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];

                VarausEntry.Text = row["varattu_pvm"].ToString();
                VahvistuspvmEntry.Text = row["vahvistus_pvm"].ToString();
                VarausAlkupvmEntry.Text = row["varattu_alkupvm"].ToString();
                VarausLoppupvmEntry.Text = row["varattu_loppupvm"].ToString();
                AsiakasIDEntry.Text = row["asiakas_id"].ToString();
                MokkiIDEntry.Text = row["mokki_id"].ToString();
            }
            else
            {
                await DisplayAlert("Ei tuloksia", "Hakusi ei palauttanut tietoja", "OK");
            }


        }
        catch (Exception ex)
        {
            // Show an error if the query failed
            await DisplayAlert("Virhe", $"Haku epäonnistui: {ex.Message}", "OK");
        }



    }
}