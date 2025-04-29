namespace Ohjelmistotuotanto_1;

public partial class PalveluidenHallinta : ContentPage
{
	public PalveluidenHallinta()
	{
		InitializeComponent();
	}

    private void PalveluCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // palvelu listasta painettu
    }


    private void LisaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // lis‰‰ uusi palvelu
    }

    private void MuokkaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa valittua palvelua
    }

    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // alue valittu valikosta ja laitta sen id:n entry
    }

    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitsee mink‰ suhteen haetaan
    }

    private void Palveluhaku_SearchButtonPressed(object sender, EventArgs e)
    {
        // Hakee palvelun valitun asian suhteen
    }

    private void PoistaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // poistaa valitun palvelun
    }
}