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

    private void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa SQ�
    }

    private void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa olemassa olevan SQL
    }


    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // kun valitaan alue_Id listasta, laita arvo entry kentt��n
    }

    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitsee mink� suhteen haetaan
    }

    private void Mokki_SearchButtonPressed(object sender, EventArgs e)
    {
            // hakee M�kin valitun asian suhteen
    }
}