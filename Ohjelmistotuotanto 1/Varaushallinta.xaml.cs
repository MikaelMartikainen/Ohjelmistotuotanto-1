namespace Ohjelmistotuotanto_1;

public partial class Varaushallinta : ContentPage
{
	public Varaushallinta()
	{
		InitializeComponent();
	}

    private void VarausCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Valitsee varauksen painaessa j alaittaa sen tiedot entry kentt‰‰n.
    }

    private void VarausTekoSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii varaukseia sen tekop‰iv‰n mukaan
    }

    private void LisaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // Lis‰‰ uuden varauksen
    }

    private void MuokkaaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa varausta
    }

    private void PoistaVarausNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa varauksen
    }

    private void VahvistuspvmSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii varauksia vahvistusop‰iv‰m‰‰r‰n perusteella
    }

    private void VarausAlkupvmSB_SearchButtonPressed(object sender, EventArgs e)
    {
        //Etsi varauksia alkup‰iv‰ m‰‰r‰n perusteella
    }

    private void VarausLoppupvmSB_SearchButtonPressed(object sender, EventArgs e)
    {
        //Etsii varauksia loopumis p‰iv‰m‰‰r‰n perusteella
    }

    private void AsiakasCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Kun asiakas listasta valitaan asiakas niin laittaa asiakkaan id:n entry kentt‰‰n
    }

    private void AsiakasIDSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // etsii varauksia asiakas id:n perusteella
    }

    private void MokkiIDSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii varauksia mˆkki id:n perusteella
    }

    private void MokkiCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Kuhn mˆkkilistasta valitaan mˆkki, laittaa sen id:n entry kentt‰‰n
    }
}