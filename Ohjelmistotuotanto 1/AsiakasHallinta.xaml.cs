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
        // lis�� asiakas
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
        // etsi asiakasta etunimell�
    }

    private void SukunimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakasta sukunimell�
    }

    private void LahiosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakasta osoitteella
    }

    private void S�hk�ppostiosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii asiakkaan s�hk�postilla
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