namespace Ohjelmistotuotanto_1;

public partial class MokkiHallinta : ContentPage
{
	public MokkiHallinta()
	{
		InitializeComponent();
	}

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Asia valitaan mökki listasta ja laitetaan entry kenttiin
    }

 
    private void LisaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // lisää uuden mökin SQL:ään
    }
    private void MokkinimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin nimen perusteella
    }

    private void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa SQÖ
    }

    private void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa olemassa olevan SQL
    }

    private void HintaSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin Hinnan perusteella
    }

    private void KatuosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin katuosoitteen perusteella
    }

    private void KuvausSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin kuvauksen perusteella
    }

    private void HenkiloMaaraSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin henkilömäärän perusteella
    }

    private void VarusteluSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin Varustelun perusteella
    }

    private void PostinumeroSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin Postinumeron perusteella
    }

    private void AlueIDSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin AlueID:n perusteella
    }

    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // kun valitaan alue_Id listasta, laita arvo entry kenttään
    }
}