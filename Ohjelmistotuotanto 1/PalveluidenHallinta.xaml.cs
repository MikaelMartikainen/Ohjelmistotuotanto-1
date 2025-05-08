using System.Data;

namespace Ohjelmistotuotanto_1;

public partial class PalveluidenHallinta : ContentPage
{
    string nimi;
    string kuvaus;
    string sHinta;
    string sAlv;
    string sAlueId; 

    private DatabaseHelper dbHelper;
    private DataTable PalveluData;
    private DataTable AlueData;
    private int PalveluID = -1;
    public PalveluidenHallinta()
	{
		InitializeComponent();
	}

    private async void LoadPalveluData()
    {
        try
        {
            // Fetch all areas from the database
            PalveluData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`palvelu` ORDER BY palvelu_id");
            PalveluCollectionView.ItemsSource = PalveluData.DefaultView;

            AlueData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`alue` ORDER BY alue_id");
            AlueCollectionView.ItemsSource = AlueData.DefaultView;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Tietojen lataus ep‰onnistui: {ex.Message}", "OK");
        }
    }


    private void PalveluCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // tarkistetaan valinta
        if (e.CurrentSelection.Count > 0)
        {
            
            var selectedItem = e.CurrentSelection[0] as DataRowView;

            if (selectedItem != null)
            {
                //laitetaan tiedot kenttiin
                PalveluEntry.Text = selectedItem.Row["nimi"].ToString();
                KuvausEntry.Text = selectedItem.Row["kuvaus"].ToString();
                HintaEntry.Text = selectedItem.Row["hinta"].ToString();
                alvEntry.Text = selectedItem.Row["alv"].ToString();
                AlueIDEntry.Text = selectedItem.Row["alue_id"].ToString();
            }
        }
    }


    private async void LisaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // alustetaan muuttujat
         nimi = PalveluEntry.Text;
         kuvaus = KuvausEntry.Text;
         sHinta = HintaEntry.Text;
         sAlv = alvEntry.Text;
         sAlueId = AlueIDEntry.Text;

        // Varmistetaan ett‰ kent‰t ovat t‰ynn‰
        if (string.IsNullOrEmpty(nimi) || string.IsNullOrEmpty(kuvaus) ||
            string.IsNullOrEmpty(sHinta) || string.IsNullOrEmpty(sAlv) ||
            string.IsNullOrEmpty(sAlueId))
        {
            await DisplayAlert("Virhe", "Kaikki kent‰t tulee t‰ytt‰‰.", "OK");
            return;
        }

        // Parse numeroille
        if (!decimal.TryParse(sHinta, out decimal hinta) ||
            !decimal.TryParse(sAlv, out decimal alv) ||
            !int.TryParse(sAlueId, out int alueId))
        {
            await DisplayAlert("Virhe", "Tarkista syˆtetyt tiedot.", "OK");
            return;
        }

        // SQL 
        var parameters = new Dictionary<string, object>
    {
        { "@nimi", nimi },
        { "@kuvaus", kuvaus },
        { "@hinta", hinta },
        { "@alv", alv },
        { "@alue_id", alueId }
    };


        try
        {   int iVastaavatRivit = await dbHelper.ExecuteNonQueryAsync("INSERT INTO vn.palvelu (nimi, kuvaus, hinta, alv, alue_id) VALUES (@nimi, @kuvaus, @hinta, @alv, @alue_id)", parameters);

            if (iVastaavatRivit>0)
            { LoadPalveluData();
                await DisplayAlert("Onnistui", "Palvelu lis‰tty", "OK");
            }

            else
            {
                await DisplayAlert("Virhe", "Plavelun lis‰‰minen ep‰onnistui", "OK");
            }


          
        }

        catch (Exception ex)
        {
            // N‰ytet‰‰n virheilmoitus, jos lis‰ys ep‰onnistuu
            await DisplayAlert("Virhe", $"Lis‰ys ep‰onnistui: {ex.Message}", "OK");
        }

    }


    private async void MuokkaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        if (PalveluID == -1)
        {
            await DisplayAlert("Huomio", "Valitse ensin muokattava palvelu", "OK");
            return;
        }

            //varmistetaan ett‰ kent‰t ovat t‰ynn‰
        if (string.IsNullOrWhiteSpace(PalveluEntry.Text) || string.IsNullOrWhiteSpace(KuvausEntry.Text) ||
            string.IsNullOrWhiteSpace(HintaEntry.Text) || string.IsNullOrWhiteSpace(alvEntry.Text) ||
            string.IsNullOrWhiteSpace(AlueIDEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kent‰t tulee t‰ytt‰‰", "OK");
            return;
        }

        try
        {
            var tiedot = new Dictionary<string, object>
        {
            { "@nimi", PalveluEntry.Text },
            { "@kuvaus", KuvausEntry.Text },
            { "@hinta", HintaEntry.Text },
            { "@alv", alvEntry.Text },
            { "@alue_id", AlueIDEntry.Text },
            { "@palvelu_id", PalveluID }  
        };

            // SQL 
            await dbHelper.ExecuteNonQueryAsync(
                "UPDATE `vn`.`palvelu` " +"SET nimi = @nimi, kuvaus = @kuvaus, hinta = @hinta, alv = @alv, alue_id = @alue_id WHERE palvelu_id = @palvelu_id", tiedot);

            await DisplayAlert("Onnistui", "Palvelu p‰ivitetty onnistuneesti", "OK");

            LoadPalveluData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"P‰ivitys ep‰onnistui: {ex.Message}", "OK");
        }
    }


    private async void PoistaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // valitaan poistettava palvelu
        var poistettavaPalvelu = PalveluCollectionView.SelectedItem as DataRowView;

        if (poistettavaPalvelu == null)
        {
            await DisplayAlert("Virhe", "Valitse poistettava palvelu.", "OK");
            return;
        }

        // vahvistetaan poistaminen
        var confirm = await DisplayAlert("Vahvista poisto", "Oletko varma, ett‰ haluat poistaa t‰m‰n palvelun?", "Kyll‰", "Ei");
        if (!confirm)
        {
            return;
        }

        // poistettavan palvelun id
        int palveluId = Convert.ToInt32(poistettavaPalvelu.Row["palvelu_id"]);

        // Valmistellaan sql poistamaan kyseinen palvelu
        var parameters = new Dictionary<string, object>
            {
                { "@palvelu_id", palveluId }
            };

        try
        {
            // poistetaan palvelu
            int rowsAffected = await dbHelper.ExecuteNonQueryAsync("DELETE FROM `vn`.`palvelu` WHERE palvelu_id = @palvelu_id", parameters);

            if (rowsAffected > 0)
            {
                await DisplayAlert("Onnistui", "Palvelu poistettu onnistuneesti.", "OK");
                LoadPalveluData(); // p‰ivitet‰‰n sivu
            }
            else
            {
                await DisplayAlert("Virhe", "Poisto ep‰onnistui.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Poisto ep‰onnistui: {ex.Message}", "OK");
        }
    }



    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;

            if (selectedItem != null)
            {
                AlueIDEntry.Text = selectedItem.Row["alue_id"].ToString();
            }
        }
    }


    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitaan mit‰ haetaan
        var valittuEhto = hakuvalitsija.SelectedItem as string;

        if (!string.IsNullOrEmpty(valittuEhto))
        {

            // suoritetaan haku
            Palveluhaku_SearchButtonPressed(valittuEhto);
        }
    }




    private async void Palveluhaku_SearchButtonPressed(string valittuHaku)
    {
        string query = "SELECT * FROM palvelu WHERE ";

        if (valittuHaku == "Nimi")
        {
            query += $"nimi LIKE '%{PalveluEntry.Text}%'";
        }
        else if (valittuHaku == "Kuvaus")
        {
            query += $"kuvaus LIKE '%{KuvausEntry.Text}%'";
        }
        else if (valittuHaku == "Hinta")
        {
            query += $"hinta = {HintaEntry.Text}";
        }
        else if (valittuHaku == "Alv")
        {
            query += $"alv = {alvEntry.Text}";
        }
        else if (valittuHaku == "AlueID")
        {
            query += $"alue_id = {AlueIDEntry.Text}";
        }
        else
        {
            await DisplayAlert("Virhe", "Tuntematon hakuehto", "OK");
            return;
        }

        try
        {
            var tulokset = await dbHelper.GetDataAsync(query);
            PalveluCollectionView.ItemsSource = tulokset.DefaultView;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Haku ep‰onnistui: {ex.Message}", "OK");
        }
    }
   
}