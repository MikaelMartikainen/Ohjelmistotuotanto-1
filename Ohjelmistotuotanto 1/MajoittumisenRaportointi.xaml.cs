using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class MajoittumisenRaportointi : ContentPage
    {
        // Lista alueista Pickeriä varten
        public ObservableCollection<Alue> Alueet { get; set; }

        // Valittu alue
        public Alue ValittuAlue { get; set; }

        public MajoittumisenRaportointi()
        {
            InitializeComponent();

            // Alustetaan alueet pickeriin, tässä nyt esimerkkinä pari aluetta lisätään myöhemmin kaikki haluttavat
            Alueet = new ObservableCollection<Alue>
            {
                new Alue { AlueNimi = "Helsinki" },
                new Alue { AlueNimi = "Tampere" },
             
            };

            // Asetetaan alustava aikajakso 
            AloitusPvm.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            LoppuPvm.Date = DateTime.Now;

            // binding pickerin tietolähteelle
            AluePicker.ItemsSource = Alueet;
            AluePicker.ItemDisplayBinding = new Binding("AlueNimi");

            // Luodaan tyhjä raportti näkyviin
            RaporttiCollectionView.ItemsSource = new ObservableCollection<RaporttiRivi>();
        }

        // Lataa Raportti -napin käsittelijä
        private async void LataaRaporttiButton_Clicked(object sender, EventArgs e)
        {
            if (AluePicker.SelectedItem == null)
            {
                await DisplayAlert("Virhe", "Valitse alue ennen raportin lataamista.", "OK");
                return;
            }

            // Tässä normaalisti haettaisiin tietokannasta tiedot

            // Näytetään testi-dataa CollectionView:ssa
            var raporttiData = new ObservableCollection<RaporttiRivi>
            {
                new RaporttiRivi { RaporttiTiedot = "01.04.2025 - 02.04.2025: 10 yötä" },
                new RaporttiRivi { RaporttiTiedot = "03.04.2025 - 05.04.2025: 5 yötä" }
            };

            RaporttiCollectionView.ItemsSource = raporttiData;
        }
    }

    // Luokka alueille
    public class Alue
    {
        public string AlueNimi { get; set; }
    }

    // Luokka raportin riveille
    public class RaporttiRivi
    {
        public string RaporttiTiedot { get; set; }
    }
}
