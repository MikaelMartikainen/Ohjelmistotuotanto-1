namespace Ohjelmistotuotanto_1;

public partial class Varaushallinta : ContentPage
{
	public Varaushallinta()
	{
		InitializeComponent();
	}

    private void VarausCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Asia valitaan varaus listasta ja laitetaan entry kenttiin
    }


    private void LisaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // Lis�� uuden varauksen
    }

    private void MuokkaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa varausta
    }

    private void PoistaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa varauksen 
    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Kun asiakas listasta valitaan asiakas niin laittaa asiakkaan id:n entry kentt��n
    }

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Kuhn m�kkilistasta valitaan m�kki, laittaa sen id:n entry kentt��n
    }

    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitsee mink� suhteen haetaan
    }

    private void VarausHaku_SearchButtonPressed(object sender, EventArgs e)
    {
        // Hakee varauksen valitun asian suhteen
    }
}