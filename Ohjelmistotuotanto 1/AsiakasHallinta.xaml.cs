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
        {       //ladataan asiakkaat ja näytetään niiden lista collectionViewssä
            asiakasData = await dbHelper.GetDataAsync("SELECT * FROM vn.asiakas ORDER BY sukunimi");
            AsiakasCollectionView.ItemsSource = asiakasData.DefaultView;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Tietojen lataus epäonnistui: {ex.Message}", "OK");
        }
    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)       //kun listasta valitaan asiakas
        {
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)           //jos asiakkaasta löytyy tietoja, ne ladataan omiin kenttiinsä
            {
                selectedAsiakasId = Convert.ToInt32(selectedItem.Row["asiakas_id"]);
                EtunimiEntry.Text = selectedItem.Row["etunimi"].ToString();
                SukunimieEntry.Text = selectedItem.Row["sukunimi"].ToString();
                LahiosoiteEntry.Text = selectedItem.Row["lahiosoite"].ToString();
                SähköpostiosoiteEntry.Text = selectedItem.Row["email"].ToString();
                PuhelinnumeroEntry.Text = selectedItem.Row["puhelinnro"].ToString();
                PostinumeroEntry.Text = selectedItem.Row["postinro"].ToString();
            }
            
        }
    }

    private async void LisaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // Tarkistetaan, että kaikki kentät on täytetty
        if (string.IsNullOrWhiteSpace(EtunimiEntry.Text) || string.IsNullOrWhiteSpace(SukunimieEntry.Text) ||
            string.IsNullOrWhiteSpace(LahiosoiteEntry.Text) || string.IsNullOrWhiteSpace(PuhelinnumeroEntry.Text) ||
            string.IsNullOrWhiteSpace(PostinumeroEntry.Text) || string.IsNullOrWhiteSpace(SähköpostiosoiteEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kentät täytyy täyttää.", "OK");
            return;
        }
        try
        {
            var parameters = new Dictionary<string, object>
        {
            { "@etunimi", EtunimiEntry.Text },
            { "@sukunimi", SukunimieEntry.Text },
            { "@lahiosoite", LahiosoiteEntry.Text },
            { "@email", SähköpostiosoiteEntry.Text },
            { "@puhelinnro", PuhelinnumeroEntry.Text },
            { "@postinro", PostinumeroEntry.Text },
        };

            // SQL-lause, joka lisää uuden asiakkaan tietokantaan
            dbHelper.ExecuteNonQueryAsync("INSERT INTO vn.asiakas (etunimi, sukunimi, lahiosoite, sahkoposti, puhelinnumero, postinumero) " + "VALUES (@etunimi, @sukunimi, @lahiosoite, @email, @puhelinnro, @postinro)", parameters);

            // Päivitetään asiakaslista näkymässä, jotta uusi asiakas näkyy 
            LoadAsiakasData();

            // Näytetään käyttäjälle ilmoitus, että lisäys onnistui
            await DisplayAlert("Onnistui", "Asiakas lisätty onnistuneesti", "OK");
           

        }

        catch (Exception ex)
        {
            // Näytetään virheilmoitus, jos lisäys epäonnistuu
            await DisplayAlert("Virhe", $"Lisäys epäonnistui: {ex.Message}", "OK");
        }

    }
   


    private async void PoistaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // Tarkistetaan onko asiakas valittu
        if (selectedAsiakasId == -1)
        {
            await DisplayAlert("Huomio", "Valitse poistettava asiakas", "OK");
            return; // Keskeytetään jos ei ole 
        }
        bool confirm = await DisplayAlert("Varmistus", $"Haluatko varmasti poistaa asiakkaan: {EtunimiEntry.Text} {SukunimieEntry.Text}?", "Kyllä", "Ei");
        if (!confirm)
            return; // Jos käyttäjä painaa "Ei" niin keskeytetään

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

                // Ilmoitetaan onnistuneesta poistosta käyttäjälle
                await DisplayAlert("Onnistui", "Asiakas poistettu", "OK");

                //tyhjennetään kentät 
                EtunimiEntry.Text = string.Empty;
                SukunimieEntry.Text = string.Empty;
                LahiosoiteEntry.Text = string.Empty;
                PostinumeroEntry.Text = string.Empty;
                PuhelinnumeroEntry.Text = string.Empty;
                SähköpostiosoiteEntry.Text = string.Empty;
                selectedAsiakasId = -1;


                LoadAsiakasData(); // Ladataan päivitetyt asiakastiedot

            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poisto epäonnistui: {ex.Message}", "OK");
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
        // Tarkistetaan, että kaikki kentät on täytetty
        if (string.IsNullOrWhiteSpace(EtunimiEntry.Text) || string.IsNullOrWhiteSpace(SukunimieEntry.Text) ||
            string.IsNullOrWhiteSpace(LahiosoiteEntry.Text) || string.IsNullOrWhiteSpace(PuhelinnumeroEntry.Text) ||
            string.IsNullOrWhiteSpace(PostinumeroEntry.Text))
        {
            await DisplayAlert("Huomio", "Kaikki kentät täytyy täyttää.", "OK");
            return;
        }


        try
        {
            //valitaan kaikki kentät
            var parameters = new Dictionary<string, object>
        {
            { "@asiakas_id", selectedAsiakasId },
            { "@etunimi", EtunimiEntry.Text },
            { "@sukunimi", SukunimieEntry.Text },
            { "@email", SähköpostiosoiteEntry.Text },
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

            await DisplayAlert("Onnistui", "Asiakastiedot päivitetty", "OK");
            LoadAsiakasData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Päivitys epäonnistui: {ex.Message}", "OK");
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
                SähköpostiosoiteEntry.Text = selectedItem.Row["email"].ToString();
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
            case 2: // Lähiosoite
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
            await DisplayAlert("Virhe", $"Haku epäonnistui: {ex.Message}", "OK");
        }
    }

    private void Hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}