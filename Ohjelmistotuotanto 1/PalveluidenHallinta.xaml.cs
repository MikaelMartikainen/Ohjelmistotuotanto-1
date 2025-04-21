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

    private void PalveluNimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi palveluista palvelun nimell‰
    }

    private void LisaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // lis‰‰ uusi palvelu
    }

    private void MuokkaaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa valittua palvelua
    }

    private void PoistaPalveluNappi_Clicked(object sender, EventArgs e)
    {
        // poista valittu palvelu
    }

    private void KuvausSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi palvelua kuvauksella
    }

    private void alvSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi palvelua alvilla
    }

    private void HintaSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi palvelua hinnalla
    }

    private void AlueIDSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi palvelua alue id:ll‰
    }

    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // alue valittu valikosta ja laitta sen id:n entry
    }
}