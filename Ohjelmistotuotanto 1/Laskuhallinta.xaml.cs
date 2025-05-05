using System;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class Laskuhallinta : ContentPage
    {
        public Laskuhallinta()
        {
            InitializeComponent();
            // T�nne voidaan lis�t� koodi, joka hakee laskut tietokannasta
        }

        // T�h�n koodia kun valitaan lasku listalta (xaml puolella LaskuCollectionView) 
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
                // Laskua ei valittuna = nappi ei k�yt�ss�
               
                PoistaLaskuButton.IsEnabled = false;
            }
        }

        // Luo PDF -painikkeen tapahtumank�sittelij�
        private async void LuoPdfButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            // Selvitet��n, kumpi laskutustapa on valittu
            string laskutustapa = TulostusPaperilaskuRadio.IsChecked ? "Paperilasku" : "S�hk�postilasku";

            // T�ss� kohdassa luodaan PDF laskusta
            // Lis�� koodi PDF:n luontiin ja tallentamiseen

            await DisplayAlert("PDF Luotu", $"PDF on luotu muodossa: {laskutustapa}", "OK");
        }

        // Laskun lis��minen -tapahtumank�sittelij�
        private async void LisaaLaskuButton_Clicked(object sender, EventArgs e)
        {
            // T�ss� koodi uuden laskun lis��miseksi tietokantaan
            // Esimerkiksi voidaan ohjata k�ytt�j� toiseen n�kym��n tai avata lomake laskun t�ytt�mist� varten

            await DisplayAlert("Lasku Lis�tty", "Uusi lasku on lis�tty.", "OK");
        }

        
        // Laskun poistaminen -tapahtumank�sittelij�
        private async void PoistaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse poistettava lasku.", "OK");
                return;
            }

            // T�h�n koodi, joka poistaa valitun laskun tietokannasta
            // Esimerkiksi voit poistaa laskun tietokannasta ja p�ivitt�� n�kym�n

            await DisplayAlert("Lasku Poistettu", "Valittu lasku on poistettu.", "OK");
        }

        // Tulosta Lasku -tapahtumank�sittelij�
        private async void TulostaLaskuButton_Clicked(object sender, EventArgs e)
        {
            if (LaskuCollectionView.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse lasku ensin.", "OK");
                return;
            }

            // Selvitet��n valittu tulostustapa
            string tulostustapa = TulostusSahkopostilaskuRadio.IsChecked ? "Paperilasku" : "S�hk�postilasku";

            // T�ss� kohdassa lis��t koodin, joka tulostaa laskun tai l�hett�� sen s�hk�postitse
            // Esimerkiksi voi avata PDF-luontitoiminnon tai l�hett�� s�hk�postin

            await DisplayAlert("Tulostus", $"Lasku tulostettu muodossa: {tulostustapa}", "OK");
        }
    }
}
