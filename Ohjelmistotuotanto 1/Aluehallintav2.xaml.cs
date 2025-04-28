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
			var parameters = new Dictionary<string, object>
			{
				{ "@nimi", AlueEntry.Text }
			};
			
			await dbHelper.ExecuteNonQueryAsync("INSERT INTO `vn`.`alue` (nimi) VALUES (@nimi)", parameters);
			
			await DisplayAlert("Onnistui", "Uusi alue lisätty onnistuneesti", "OK");
			
			// Clear the entry and reload data
			AlueEntry.Text = string.Empty;
			selectedAlueId = -1;
			LoadAlueData();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Virhe", $"Lisääminen epäonnistui: {ex.Message}", "OK");
		}
	}
	
	private async void HaeNappi_Clicked(object sender, EventArgs e)
	{
		try
		{
			string searchTerm = AlueSearchBar.Text;
			
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				// If search term is empty, load all data
				LoadAlueData();
				return;
			}
			
			var parameters = new Dictionary<string, object>
			{
				{ "@searchTerm", $"%{searchTerm}%" }
			};
			
			alueData = await dbHelper.GetDataWithParamsAsync(
				"SELECT * FROM `vn`.`alue` WHERE nimi LIKE @searchTerm ORDER BY nimi", 
				parameters);
				
			AlueCollectionView.ItemsSource = alueData.DefaultView;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Virhe", $"Haku epäonnistui: {ex.Message}", "OK");
		}
	}
}
