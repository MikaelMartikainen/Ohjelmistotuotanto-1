using Mysqlx.Crud;
using System.Data;

namespace Ohjelmistotuotanto_1;

public partial class MokkiHallinta : ContentPage
{

    private DatabaseHelper dbHelper;
    private DataTable MokkiData;
    private DataTable AlueData;
    private int mokkiID = -1;

    public MokkiHallinta()
	{
		InitializeComponent();
        dbHelper = new DatabaseHelper();
        LoadMokkiData();
    }

    private async void LoadMokkiData()
    {
        try
        {
            // Fetch all areas from the database
            MokkiData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`mokki` ORDER BY mokki_id");
            MokkiCollectionView.ItemsSource = MokkiData.DefaultView;

            AlueData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`alue` ORDER BY alue_id");
            AlueCollectionView.ItemsSource = AlueData.DefaultView;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Tietojen lataus epäonnistui: {ex.Message}", "OK");
        }
    }

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Select an item from the mokki list and put it into the entry field

        if (e.CurrentSelection.Count > 0)
        {
            // Get the selected item
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)
            {
                MokkinimiEntry.Text = selectedItem.Row["mokkinimi"].ToString();
                KatuosoiteEntry.Text = selectedItem.Row["katuosoite"].ToString();
                HintaEntry.Text = selectedItem.Row["hinta"].ToString();
                KuvausEntry.Text = selectedItem.Row["kuvaus"].ToString();
                HenkiloMaaraEntry.Text = selectedItem.Row["henkilomaara"].ToString();
                VarusteluEntry.Text = selectedItem.Row["varustelu"].ToString();
                PostinumeroEntry.Text = selectedItem.Row["postinro"].ToString();
                AlueIDEntry.Text = selectedItem.Row["alue_id"].ToString();

                //
                mokkiID = Convert.ToInt32(selectedItem.Row["mokki_id"]);
            }
        }
    }

    private async void LisaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Add new mökki to SQL


        //Check if Mökki nimi in empty
        if (string.IsNullOrWhiteSpace(MokkinimiEntry.Text))
        {
            await DisplayAlert("Huomio", "Anna mökin nimi", "OK");
            return;
        }

        try
        {
            // Loading data from entry fields to teidot object
            var tiedot = new Dictionary<string, object>
            {
                { "@mokkinimi", MokkinimiEntry.Text },
                { "@katuosoite", KatuosoiteEntry.Text },
                { "@hinta", HintaEntry.Text },
                { "@kuvaus", KuvausEntry.Text },
                { "@henkilomaara", HenkiloMaaraEntry.Text },
                { "@varustelu", VarusteluEntry.Text },
                { "@postinro", PostinumeroEntry.Text },
                { "@alue_id", AlueIDEntry.Text },
            };

            //Running SQL querry
            await dbHelper.ExecuteNonQueryAsync("INSERT INTO `vn`.`mokki` (`alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`) " + "VALUES (@alue_id, @postinro, @mokkinimi, @katuosoite, @hinta, @kuvaus, @henkilomaara, @varustelu)", tiedot);

            //Updating list to show ner data
            LoadMokkiData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Lisääminen epäonnistui: {ex.Message}", "OK");
        }
    }

    private async void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Edit an existing mökki in sql

        if (mokkiID == -1)
        {
            await DisplayAlert("Huomio", "Valitse ensin muokattava mökki", "OK");
            return;
        }

        if(string.IsNullOrWhiteSpace(MokkinimiEntry.Text))

        {
            await DisplayAlert("Huomio", "Anna mökin nimi", "OK");
            return;
        }

        try
        {
            var tiedot = new Dictionary<string, object>
            {
                { "@mokkinimi", MokkinimiEntry.Text },
                { "@katuosoite", KatuosoiteEntry.Text },
                { "@hinta", HintaEntry.Text },
                { "@kuvaus", KuvausEntry.Text },
                { "@henkilomaara", HenkiloMaaraEntry.Text },
                { "@varustelu", VarusteluEntry.Text },
                { "@postinro", PostinumeroEntry.Text },
                { "@alue_id", AlueIDEntry.Text },
                { "@mokki_id", mokkiID }
            };

            //Running the SQL querry
            await dbHelper.ExecuteNonQueryAsync("UPDATE `vn`.`mokki` " +"SET alue_id = @alue_id, " +"postinro = @postinro, " +"mokkinimi = @mokkinimi, " +"katuosoite = @katuosoite, " +"hinta = @hinta, " +"kuvaus = @kuvaus, " +"henkilomaara = @henkilomaara, " +"varustelu = @varustelu " +"WHERE mokki_id = @mokki_id", tiedot);


            await DisplayAlert("Onnistui", "Alue päivitetty onnistuneesti", "OK");

            //Updating list to show ner data
            LoadMokkiData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Päivittäminen epäonnistui: {ex.Message}", "OK");
        }
    }

    private async void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Delete an existing mookki in sql

        if (mokkiID == -1)
        {
            await DisplayAlert("Huomio", "Valitse ensin poistettava mökki", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Varmista", $"Haluatko varmasti poistaa alueen: {MokkinimiEntry.Text}?", "Kyllä", "Ei");
        if (confirm)
        {
            try
            {
                var tiedot = new Dictionary<string, object>
                {
                    { "@mokki_id", mokkiID }
                };

                await dbHelper.ExecuteNonQueryAsync("DELETE FROM `vn`.`mokki` WHERE mokki_id = @mokki_id", tiedot);

                await DisplayAlert("Onnistui", "Alue poistettu onnistuneesti", "OK");

                //Empty all entryfields, refers list wiht new data
                MokkinimiEntry.Text = string.Empty;
                KatuosoiteEntry.Text = string.Empty;
                HintaEntry.Text = string.Empty;
                KuvausEntry.Text = string.Empty;
                HenkiloMaaraEntry.Text = string.Empty;
                VarusteluEntry.Text = string.Empty;
                PostinumeroEntry.Text = string.Empty;
                AlueIDEntry.Text = string.Empty;
                mokkiID = -1;
                LoadMokkiData();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poistaminen epäonnistui: {ex.Message}", "OK");
            }
        }
    }

    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // When an area is selected from the list, put the value into the entry field

        if (e.CurrentSelection.Count > 0)
        {
            // Get the selected item
            var selectedItem = e.CurrentSelection[0] as DataRowView;
            if (selectedItem != null)
            {
                //Change the AlueIDEntry to the selected entry from SQL
                AlueIDEntry.Text = selectedItem.Row["alue_id"].ToString();
            }
        }
    }

    private async void Mokki_SearchButtonPressed(object sender, EventArgs e)
    {
        // Search for the selected item

        var haku = new Dictionary<string, string>
        {
            { "Mökin nimi", "mokkinimi" },
            { "Katuosoite", "katuosoite" },
            { "Kuvaus", "kuvaus" },
            { "Varustelu", "varustelu" },
            { "Postinumero", "postinro" },
            { "AlueID", "alue_id" }
        };


        string searchText = Mokki.Text;
        string selectedCategory = hakuvalitsija.SelectedItem.ToString();

        //Cheching if search field is empty
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

        //Runnig the SQL quearry
        DataTable result = await dbHelper.GetDataAsync($"SELECT * FROM `vn`.`mokki` WHERE {columnName} LIKE '{searchText}%'");

        //Placeing the searched data into the entry fields
        if (result.Rows.Count > 0)
        {
            DataRow row = result.Rows[0];

            MokkinimiEntry.Text = row["mokkinimi"].ToString();
            KatuosoiteEntry.Text = row["katuosoite"].ToString();
            HintaEntry.Text = row["hinta"].ToString();
            KuvausEntry.Text = row["kuvaus"].ToString();
            HenkiloMaaraEntry.Text = row["henkilomaara"].ToString();
            VarusteluEntry.Text = row["varustelu"].ToString();
            PostinumeroEntry.Text = row["postinro"].ToString();
            AlueIDEntry.Text = row["alue_id"].ToString();
        }
        else
        {
            await DisplayAlert("Ei tuloksia", "Hakusi ei palauttanut tietoja", "OK");
        }


    }
}