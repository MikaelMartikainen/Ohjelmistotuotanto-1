namespace Ohjelmistotuotanto_1;

public partial class MokkiHallinta : ContentPage
{
	public MokkiHallinta()
	{
		InitializeComponent();
	}

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Asia valitaan m�kki listasta ja laitetaan entry kenttiin
    }

 
    private void LisaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // lis�� uuden m�kin SQL:��n
    }
    private void MokkinimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin nimen perusteella
    }

    private void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa SQ�
    }

    private void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa olemassa olevan SQL
    }

    private void HintaSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin Hinnan perusteella
    }

    private void KatuosoiteSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin katuosoitteen perusteella
    }

    private void KuvausSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin kuvauksen perusteella
    }

    private void HenkiloMaaraSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin henkil�m��r�n perusteella
    }

    private void VarusteluSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin Varustelun perusteella
    }

    private void PostinumeroSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin Postinumeron perusteella
    }

    private void AlueIDSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii m�kin AlueID:n perusteella
    }

    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // kun valitaan alue_Id listasta, laita arvo entry kentt��n
    }
}