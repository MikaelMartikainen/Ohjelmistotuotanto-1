using System.Data.SqlTypes;
using System.Data;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1;

public partial class Aluehallintav2 : ContentPage
{
	private DatabaseHelper dbHelper;
	private DataTable alueData;
	private int selectedAlueId = -1;

	public Aluehallintav2()
	{
		InitializeComponent();
		dbHelper = new DatabaseHelper();
		LoadAlueData();
	}

	private async void LoadAlueData()
	{
		try
		{
			// Fetch all areas from the database
			alueData = await dbHelper.GetDataAsync("SELECT * FROM `vn`.`alue` ORDER BY nimi");
			AlueCollectionView.ItemsSource = alueData.DefaultView;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Virhe", $"Tietojen lataus epäonnistui: {ex.Message}", "OK");
		}
	}

	private void NotesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.Count > 0)
		{
			// Get the selected item
			var selectedItem = e.CurrentSelection[0] as DataRowView;
			if (selectedItem != null)
			{
				// Store the selected area ID and display the name in the entry field
				selectedAlueId = Convert.ToInt32(selectedItem.Row["alue_id"]);
				AlueEntry.Text = selectedItem.Row["nimi"].ToString();
			}
		}
	}

	private async void PoistaNappi_Clicked(object sender, EventArgs e)
	{
		if (selectedAlueId == -1)
		{
			await DisplayAlert("Huomio", "Valitse ensin poistettava alue", "OK");
			return;
		}

		bool confirm = await DisplayAlert("Varmista", $"Haluatko varmasti poistaa alueen: {AlueEntry.Text}?", "Kyllä", "Ei");
		if (confirm)
		{
			try
			{
				var parameters = new Dictionary<string, object>
				{
					{ "@id", selectedAlueId }
				};
				
				await dbHelper.ExecuteNonQueryAsync("DELETE FROM `vn`.`alue` WHERE alue_id = @id", parameters);
				
				await DisplayAlert("Onnistui", "Alue poistettu onnistuneesti", "OK");
				
				// Clear selection and reload data
				AlueEntry.Text = string.Empty;
				selectedAlueId = -1;
				LoadAlueData();
			}
			catch (Exception ex)
			{
				await DisplayAlert("Virhe", $"Poistaminen epäonnistui: {ex.Message}", "OK");
			}
		}
	}

	private async void MuokkaaNappi_Clicked(object sender, EventArgs e)
	{
		if (selectedAlueId == -1)
		{
			await DisplayAlert("Huomio", "Valitse ensin muokattava alue", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(AlueEntry.Text))
		{
			await DisplayAlert("Huomio", "Anna alueen nimi", "OK");
			return;
		}

		try
		{
			var parameters = new Dictionary<string, object>
			{
				{ "@id", selectedAlueId },
				{ "@nimi", AlueEntry.Text }
			};
			
			await dbHelper.ExecuteNonQueryAsync("UPDATE `vn`.`alue` SET nimi = @nimi WHERE alue_id = @id", parameters);
			
			await DisplayAlert("Onnistui", "Alue päivitetty onnistuneesti", "OK");
			
			// Reload data to reflect changes
			LoadAlueData();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Virhe", $"Päivittäminen epäonnistui: {ex.Message}", "OK");
		}
	}

	private async void LisaaNappi_Clicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(AlueEntry.Text))
		{
			await DisplayAlert("Huomio", "Anna alueen nimi", "OK");
			return;
		}
        
        try
        {
            // Loading data from entry fields to teidot object
			
            var tiedot = new Dictionary<string, object>
            {
                { "@nimi", AlueEntry.Text}
            };
			
            //Running SQL querry
            await dbHelper.ExecuteNonQueryAsync("INSERT INTO `vn`.`alue` (`nimi`) " + "VALUES(@nimi)", tiedot);

            //Updating list to show new data
            LoadAlueData();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Lisääminen epäonnistui: {ex.Message}", "OK");
        }
    }



    private async void HaeNappi_Clicked(object sender, EventArgs e)
    {
        // Search for the selected item

        var haku = new Dictionary<string, string>
        {
            { "AlueID", "alue_id"},
            { "Alue", "nimi"}
        };


        string searchText = AlueSearchText.Text;
        string selectedCategory = Aluehakuavlitsija.SelectedItem.ToString();

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
        DataTable result = await dbHelper.GetDataAsync($"SELECT * FROM `vn`.`alue` WHERE {columnName} LIKE '{searchText}%'");

        //Placeing the searched data into the entry fields
        if (result.Rows.Count > 0)
        {
            DataRow row = result.Rows[0];

            AlueEntry.Text = row["nimi"].ToString();

        }
        else
        {
            await DisplayAlert("Ei tuloksia", "Hakusi ei palauttanut tietoja", "OK");
        }

    }
}

