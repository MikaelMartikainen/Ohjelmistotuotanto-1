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


        //Figuring shit out.
        //Kattoo tarviiko myöhemmin kun tulee db mukaan.
        string Nimi = MokkinimiEntry.Text;
        string Osoite = KatuosoiteEntry.Text;
        string Hinta = HintaEntry.Text;
        string Kuvaus = KuvausEntry.Text;
        string HenkMaara = HenkiloMaaraEntry.Text;
        string Varustelu = VarusteluEntry.Text;
        string PostiNum = PostinumeroEntry.Text;
        string Alue = AlueIDEntry.Text;

        // Temp Label
        Label TempLabel = new Label
        {

            Text = "Nimi: " + Nimi + " Osoite: " + Osoite + " Hinta: " + Hinta + "€ Henkilo Määrä: " + HenkMaara + " Alue: " + Alue + " Varustelu: " + Varustelu + " PostiNumero: " + PostiNum,
            TextColor = Colors.Black,
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Center
        };

        MokkiLayout.Children.Add(TempLabel);

    }


    private void MuokkaaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // muokkaa olemassa olevaa SQL
    }


    private void PoistaMokkiNappi_Clicked(object sender, EventArgs e)
    {
        // Poistaa olemassa olevan SQL
    }


    private void HintaUpdate(object sender, EventArgs e)
    {
        // Tee koodi joka tarkistaa hinnan olevan vain numerot
        // Estää tekstin joka ei ole numeroita

    }



    private void MokkinimiSB_SearchButtonPressed(object sender, EventArgs e)
    {
        // Etsii mökin nimen perusteella
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