namespace Ohjelmistotuotanto_1;

public partial class AsiakasHallinta : ContentPage
{
	public AsiakasHallinta()
	{
		InitializeComponent();
	}

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Valitaan asiakas
    }

    private void LisaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // lis‰‰ asiakas
    }

    private void MuokkaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa asiakasta
    }

    private void PoistaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // poista olemassa oleva asiakas
    }

    private void Hae_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakkaan valitun asian perusteella
    }

    private void Hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitsee mink‰ suhteen haetaan
    }
}