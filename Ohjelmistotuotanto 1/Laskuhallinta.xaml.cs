using System;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class Laskuhallinta : ContentPage
    {
        public Laskuhallinta()
        {
            InitializeComponent();
            // Tänne voidaan lisätä koodi, joka hakee laskut tietokannasta
        }

        // Tähän koodia kun valitaan lasku listalta (xaml puolella LaskuCollectionView) 
        private void LaskuCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tarkistetaan, onko listalta valittu joku lasku
            if (LaskuCollectionView.SelectedItem != null)
            {
                // Aktivoi Muokkaa ja Poista napit, kun listalta on valittuna lasku
               
                PoistaLaskuButton.IsEnabled = true;
            }
            else
            {
                // Laskua ei valittuna = nappi ei käytössä
               
                PoistaLaskuButton.IsEnabled = false;
            }
        }

        // Luo PDF -painikkeen tapahtumankäsittelijä
        private async void LuoPdfButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            // Selvitetään, kumpi laskutustapa on valittu
            string laskutustapa = TulostusPaperilaskuRadio.IsChecked ? "Paperilasku" : "Sähköpostilasku";

            // Tässä kohdassa luodaan PDF laskusta
            // Lisää koodi PDF:n luontiin ja tallentamiseen

            await DisplayAlert("PDF Luotu", $"PDF on luotu muodossa: {laskutustapa}", "OK");
        }

        // Laskun lisääminen -tapahtumankäsittelijä
        private async void LisaaLaskuButton_Clicked(object sender, EventArgs e)
        {
            // Tässä koodi uuden laskun lisäämiseksi tietokantaan
            // Esimerkiksi voidaan ohjata käyttäjä toiseen näkymään tai avata lomake laskun täyttämistä varten

            await DisplayAlert("Lasku Lisätty", "Uusi lasku on lisätty.", "OK");
        }

        
        // Laskun poistaminen -tapahtumankäsittelijä
        private async void PoistaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse poistettava lasku.", "OK");
                return;
            }

            // Tähän koodi, joka poistaa valitun laskun tietokannasta
            // Esimerkiksi voit poistaa laskun tietokannasta ja päivittää näkymän

            await DisplayAlert("Lasku Poistettu", "Valittu lasku on poistettu.", "OK");
        }

        // Tulosta Lasku -tapahtumankäsittelijä
        private async void TulostaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            // Selvitetään valittu tulostustapa
            string tulostustapa = TulostusSahkopostilaskuRadio.IsChecked ? "Paperilasku" : "Sähköpostilasku";

            // Tässä kohdassa lisäät koodin, joka tulostaa laskun tai lähettää sen sähköpostitse
            // Esimerkiksi voi avata PDF-luontitoiminnon tai lähettää sähköpostin

            await DisplayAlert("Tulostus", $"Lasku tulostettu muodossa: {tulostustapa}", "OK");
        }
    }
}
