using System;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnAlueidenHallintaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Aluehallintav2());

        }

        private async void OnPalveluidenHallintaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PalveluidenHallinta());
        }

        private async void OnMokkivarauksetClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MokkiHallinta());
        }

        private async void OnAsiakashallintaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AsiakasHallinta());
        }

        private async void OnLaskujenHallintaClicked(object sender, EventArgs e)
        {
            
        }

        private async void OnMajoittumisetAlueittainClicked(object sender, EventArgs e)
        {
            
        }

        private async void OnMajoittumistenRaportointiClicked(object sender, EventArgs e)
        {
         
        }

        private async void OnOstetutPalvelutClicked(object sender, EventArgs e)
        {
           
        }
    }
}
