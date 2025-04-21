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
        // lisää asiakas
    }

    private void MuokkaaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa asiakasta
    }

    private void PoistaAsiakasNappi_Clicked(object sender, EventArgs e)
    {
        // poista olemassa oleva asiakas
    }
    private void EtunimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsi asiakasta etunimellä
    }

    private void SukunimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakasta sukunimellä
    }

    private void LahiosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakasta osoitteella
    }

    private void SähköppostiosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakkaan sähköpostilla
    }

    private void PuhelinnumeroSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakkaan ppuhelin numeron perusteella
    }

    private void PostinumeroSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakkaan postinumeron perusteella
    }
}