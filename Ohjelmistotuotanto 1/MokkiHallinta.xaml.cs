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

    private void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa SQÖ
    }

    private void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa olemassa olevan SQL
    }


    private void AlueCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // kun valitaan alue_Id listasta, laita arvo entry kenttään
    }

    private void hakuvalitsija_SelectedIndexChanged(object sender, EventArgs e)
    {
        // valitsee minkä suhteen haetaan
    }

    private void Mokki_SearchButtonPressed(object sender, EventArgs e)
    {
            // hakee Mökin valitun asian suhteen
    }
}