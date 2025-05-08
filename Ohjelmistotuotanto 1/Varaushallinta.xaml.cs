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
            await DisplayAlert("Virhe", $"Tietojen lataus ep‰onnistui: {ex.Message}", "OK");
        }
    }




    private void VarausCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection[0] as DataRowView;    //kun varaus valitaan sen tiedot tulevat n‰kyviin
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


    private async void LisaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // varmistetaan ett‰ on t‰ydet tiedot 
        if (string.IsNullOrWhiteSpace(VarausEntry.Text) ||
            string.IsNullOrWhiteSpace(VahvistuspvmEntry.Text) ||
            string.IsNullOrWhiteSpace(VarausAlkupvmEntry.Text) ||
            string.IsNullOrWhiteSpace(VarausLoppupvmEntry.Text) ||
            string.IsNullOrWhiteSpace(AsiakasIDEntry.Text) ||
            string.IsNullOrWhiteSpace(MokkiIDEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kent‰t t‰ytyy t‰ytt‰‰.", "OK");
            return;
        }

        try
        {
                var tiedot = new Dictionary<string, object>
            {
                { "@varattu_pvm", VarausEntry.Text },
                { "@vahvistus_pvm", VahvistuspvmEntry.Text },
                { "@varattu_alkupvm", VarausAlkupvmEntry.Text },
                { "@varattu_loppupvm", VarausLoppupvmEntry.Text },
                { "@asiakas_id", AsiakasIDEntry.Text },
                { "@mokki_id", MokkiIDEntry.Text }
            };

            // SQL
            await dbHelper.ExecuteNonQueryAsync(
                "INSERT INTO `vn`.`varaus` (`varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`, `asiakas_id`, `mokki_id`) " +
                "VALUES (@varattu_pvm, @vahvistus_pvm, @varattu_alkupvm, @varattu_loppupvm, @asiakas_id, @mokki_id)",
                tiedot);

            
            LoadVarausData();

            await DisplayAlert("Onnistui", "Varaus lis‰tty onnistuneesti", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Varaus ep‰onnistui: {ex.Message}", "OK");
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
            await DisplayAlert("Huomio", "T‰yt‰ kaikki kent‰t ennen tallentamista", "OK");
            return;
        }

        try
        {
            // Create a dictionary with the new values to update
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



            await dbHelper.ExecuteNonQueryAsync(
            "UPDATE `vn`.`varaus` " +
            "SET varattu_pvm = @varattu_pvm, " +
            "vahvistus_pvm = @vahvistus_pvm, " +
            "varattu_alkupvm = @varattu_alkupvm, " +
            "varattu_loppupvm = @varattu_loppupvm, " +
            "asiakas_id = @asiakas_id, " +
            "mokki_id = @mokki_id " +
            "WHERE varaus_id = @varaus_id", tiedot);

            // Show a success message
            await DisplayAlert("Onnistui", "Varaus p‰ivitetty onnistuneesti", "OK");

            // Optionally, refresh the data to show the updated reservation
            LoadVarausData();
        }
        catch (Exception ex)
        {
            // Show error message if something goes wrong
            await DisplayAlert("Virhe", $"P‰ivitys ep‰onnistui: {ex.Message}", "OK");
        }
    }

    private async void PoistaVarausNappi_Clicked(object sender, EventArgs e)
    {
        if (VarausID == -1)
        {
            await DisplayAlert("Huomio", "Valitse ensin poistettava varaus", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Varmista", $"Haluatko varmasti poistaa varauksen {VarausEntry.Text}?", "Kyll‰", "Ei");
        if (confirm)
        {
            try
            {
                var tiedot = new Dictionary<string, object>
                {
                    { "@varaus_id", VarausID }
                };

                await dbHelper.ExecuteNonQueryAsync("DELETE FROM `vn`.`varaus` WHERE varaus_id = @varaus_id", tiedot);
                await DisplayAlert("Onnistui", "Varaus poistettu onnistuneesti", "OK");

                VarausEntry.Text = string.Empty;
                VahvistuspvmEntry.Text = string.Empty;
                VarausAlkupvmEntry.Text = string.Empty;
                VarausLoppupvmEntry.Text = string.Empty;
                AsiakasIDEntry.Text = string.Empty;
                MokkiIDEntry.Text = string.Empty;
                VarausID = -1;


                LoadVarausData();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poistaminen ep‰onnistui: {ex.Message}", "OK");
            }
        }

    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
            //kun asiakas valitaan, ID tuleee n‰kyvin
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
        var selectedIndex = hakuvalitsija.SelectedIndex;    //muuttuja valinnalle
        string searchField = string.Empty;

        switch (selectedIndex)    
        {
            case 0: 
                searchField = "varattu_pvm";
                break;
            case 1: 
                searchField = "vahvistus_pvm";
                break;
            case 2: 
                searchField = "varattu_alkupvm";
                break;
            case 3: 
                searchField = "varattu_loppupvm";
                break;
            case 4: 
                searchField = "asiakas_id";
                break;
            case 5: 
                searchField = "mokki_id";
                break;
            default:
                break;
        }

    }

    private async void VarausHaku_SearchButtonPressed(object sender, EventArgs e)
    {
        var haku = new Dictionary<string, string>
        {
            { "Varaus p‰iv‰", "varattu_pvm" },
            { "Vahvistusp‰iv‰m‰‰r‰", "vahvistus_pvm" },
            { "Varauksen alku", "varattu_alkupvm" },
            { "Varauksen loppu", "varattu_loppupvm" },
            { "Asiakas ID", "asiakas_id" },
            { "Mˆkki ID", "mokki_id" }
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
            await DisplayAlert("Virhe", $"Haku ep‰onnistui: {ex.Message}", "OK");
        }



    }
}