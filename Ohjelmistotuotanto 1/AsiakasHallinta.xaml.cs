using System.Data;

namespace Ohjelmistotuotanto_1;

public partial class AsiakasHallinta : ContentPage
{
    private DatabaseHelper dbHelper;
    private DataTable asiakasData;
    private int selectedAsiakasId = -1;

    public AsiakasHallinta()
    {
        InitializeComponent();
        dbHelper = new DatabaseHelper();
        LoadAsiakasData();
    }

    private async void LoadAsiakasData()
    {
        try
        {       //ladataan asiakkaat ja n�ytet��n niiden lista collectionViewss�
            asiakasData = await dbHelper.GetDataAsync("SELECT * FROM vn.asiakas ORDER BY sukunimi");
            AsiakasCollectionView.ItemsSource = asiakasData.DefaultView;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Tietojen lataus ep�onnistui: {ex.Message}", "OK");
        }
    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)       //kun listasta valitaan asiakas
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)           //jos asiakkaasta l�ytyy tietoja, ne ladataan omiin kenttiins�
            {
                selectedAsiakasId = Convert.ToInt32(selectedItem.Row["asiakas_id"]);
                EtunimiEntry.Text = selectedItem.Row["etunimi"].ToString();
                SukunimieEntry.Text = selectedItem.Row["sukunimi"].ToString();
                LahiosoiteEntry.Text = selectedItem.Row["lahiosoite"].ToString();
                S�hk�postiosoiteEntry.Text = selectedItem.Row["email"].ToString();
                PuhelinnumeroEntry.Text = selectedItem.Row["puhelinnro"].ToString();
                PostinumeroEntry.Text = selectedItem.Row["postinro"].ToString();
            }
            
        }
    }

    private async void LisaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // Tarkistetaan, ett� kaikki kent�t on t�ytetty
        if (string.IsNullOrWhiteSpace(EtunimiEntry.Text) || string.IsNullOrWhiteSpace(SukunimieEntry.Text) ||
            string.IsNullOrWhiteSpace(LahiosoiteEntry.Text) || string.IsNullOrWhiteSpace(PuhelinnumeroEntry.Text) ||
            string.IsNullOrWhiteSpace(PostinumeroEntry.Text) || string.IsNullOrWhiteSpace(S�hk�postiosoiteEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kent�t t�ytyy t�ytt��.", "OK");
            return;
        }
        try
        {
            var parameters = new Dictionary<string, object>
        {
            { "@etunimi", EtunimiEntry.Text },
            { "@sukunimi", SukunimieEntry.Text },
            { "@lahiosoite", LahiosoiteEntry.Text },
            { "@email", S�hk�postiosoiteEntry.Text },
            { "@puhelinnro", PuhelinnumeroEntry.Text },
            { "@postinro", PostinumeroEntry.Text },
        };

            // SQL-lause, joka lis�� uuden asiakkaan tietokantaan
            dbHelper.ExecuteNonQueryAsync("INSERT INTO vn.asiakas (etunimi, sukunimi, lahiosoite, sahkoposti, puhelinnumero, postinumero) " + "VALUES (@etunimi, @sukunimi, @lahiosoite, @email, @puhelinnro, @postinro)", parameters);

            // P�ivitet��n asiakaslista n�kym�ss�, jotta uusi asiakas n�kyy 
            LoadAsiakasData();

            // N�ytet��n k�ytt�j�lle ilmoitus, ett� lis�ys onnistui
            await DisplayAlert("Onnistui", "Asiakas lis�tty onnistuneesti", "OK");
           

        }

        catch (Exception ex)
        {
            // N�ytet��n virheilmoitus, jos lis�ys ep�onnistuu
            await DisplayAlert("Virhe", $"Lis�ys ep�onnistui: {ex.Message}", "OK");
        }

    }
   


    private async void PoistaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // Tarkistetaan onko asiakas valittu
        if (selectedAsiakasId == -1)
        {
            await DisplayAlert("Huomio", "Valitse poistettava asiakas", "OK");
            return; // Keskeytet��n jos ei ole 
        }
        bool confirm = await DisplayAlert("Varmistus", $"Haluatko varmasti poistaa asiakkaan: {EtunimiEntry.Text} {SukunimieEntry.Text}?", "Kyll�", "Ei");
        if (!confirm)
            return; // Jos k�ytt�j� painaa "Ei" niin keskeytet��n

        else
        {
            try
            {
                // valitaan id:seen kuuluvat kaikki tiedot
                var parameters = new Dictionary<string, object>
                {
                     { "@asiakas_id", selectedAsiakasId }
                };

                // Suoritetaan DELETE-komento tietokannassa parametrien kanssa
                await dbHelper.ExecuteNonQueryAsync("DELETE FROM `vn`.`asiakas` WHERE asiakas_id = @asiakas_id", parameters);

                // Ilmoitetaan onnistuneesta poistosta k�ytt�j�lle
                await DisplayAlert("Onnistui", "Asiakas poistettu", "OK");

                //tyhjennet��n kent�t 
                EtunimiEntry.Text = string.Empty;
                SukunimieEntry.Text = string.Empty;
                LahiosoiteEntry.Text = string.Empty;
                PostinumeroEntry.Text = string.Empty;
                PuhelinnumeroEntry.Text = string.Empty;
                S�hk�postiosoiteEntry.Text = string.Empty;
                selectedAsiakasId = -1;


                LoadAsiakasData(); // Ladataan p�ivitetyt asiakastiedot

            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poisto ep�onnistui: {ex.Message}", "OK");
            }
        }
    }



    private async void MuokkaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        if (selectedAsiakasId == -1)    //tarkistetaan onjo valittu asiakas
        {
            await DisplayAlert("Huomio", "Valitse muokattava asiakas", "OK");   //jos ei
            return;
        }
        // Tarkistetaan, ett� kaikki kent�t on t�ytetty
        if (string.IsNullOrWhiteSpace(EtunimiEntry.Text) || string.IsNullOrWhiteSpace(SukunimieEntry.Text) ||
            string.IsNullOrWhiteSpace(LahiosoiteEntry.Text) || string.IsNullOrWhiteSpace(PuhelinnumeroEntry.Text) ||
            string.IsNullOrWhiteSpace(PostinumeroEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kent�t t�ytyy t�ytt��.", "OK");
            return;
        }


        try
        {
            //valitaan kaikki kent�t
            var parameters = new Dictionary<string, object>
        {
            { "@asiakas_id", selectedAsiakasId },
            { "@etunimi", EtunimiEntry.Text },
            { "@sukunimi", SukunimieEntry.Text },
            { "@email", S�hk�postiosoiteEntry.Text },
            { "@lahiosoite", LahiosoiteEntry.Text },
            { "@puhelinnro", PuhelinnumeroEntry.Text },
            { "@postinro", PostinumeroEntry.Text }
        };
            //laitetaan kenttien tiedot DB:seen
            await dbHelper.ExecuteNonQueryAsync(
                "UPDATE `vn`.`asiakas` " +
                "SET etunimi = @etunimi, " +
                "sukunimi = @sukunimi, " +
                "lahiosoite = @lahiosoite, " +
                "sahkoposti = @email, " +
                "puhelinnumero = @puhelinnro, " +
                "postinumero = @postinro " +
                "WHERE asiakas_id = @asiakas_id",
                parameters);

            await DisplayAlert("Onnistui", "Asiakastiedot p�ivitetty", "OK");
            LoadAsiakasData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"P�ivitys ep�onnistui: {ex.Message}", "OK");
        }
    }

    private void AsiakasCollectionView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        // Kun asiakas valitaan listasta, asetetaan tiedot kenttiin
        if (e.CurrentSelection.Count > 0)
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)
            {

                EtunimiEntry.Text = selectedItem.Row["etunimi"].ToString();
                SukunimieEntry.Text = selectedItem.Row["sukunimi"].ToString();
                S�hk�postiosoiteEntry.Text = selectedItem.Row["email"].ToString();
                LahiosoiteEntry.Text = selectedItem.Row["lahiosoite"].ToString();
                PuhelinnumeroEntry.Text = selectedItem.Row["puhelinnro"].ToString();
                PostinumeroEntry.Text = selectedItem.Row["postinro"].ToString();


            }
        }
    }

    private async void Hae_SearchButtonPressed(object sender, EventArgs e)
    {
        string searchQuery = Hae.Text?.Trim(); // haetaan tekstin perusteella

        if (string.IsNullOrEmpty(searchQuery))
        {
            await DisplayAlert("Virhe", "Anna hakusana", "OK");
            return;
        }

        // SQL kyselyiden luonti
        string query = string.Empty;

        switch (Hakuvalitsija.SelectedIndex)
        {
            case 0: // Etunimi
                query = $"SELECT * FROM vn.asiakas WHERE etunimi LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            case 1: // Sukunimi
                query = $"SELECT * FROM vn.asiakas WHERE sukunimi LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            case 2: // L�hiosoite
                query = $"SELECT * FROM vn.asiakas WHERE lahiosoite LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            case 3: // Puhelinnumero
                query = $"SELECT * FROM vn.asiakas WHERE puhelinnro LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            case 4: // Postinumero
                query = $"SELECT * FROM vn.asiakas WHERE postinro LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            case 5: //sposti
                query = $"SELECT * FROM vn.asiakas WHERE email LIKE '%{searchQuery}%' ORDER BY sukunimi";
                break;
            default:
                await DisplayAlert("Virhe", "Valitse hakukriteeri", "OK");
                return;
        }

        try
        {
            // haetaan haun perusteella
            asiakasData = await dbHelper.GetDataAsync(query);
            AsiakasCollectionView.ItemsSource = asiakasData.DefaultView; // tilos collectionViewhin
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Haku ep�onnistui: {ex.Message}", "OK");
        }
    }

    private void Hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}