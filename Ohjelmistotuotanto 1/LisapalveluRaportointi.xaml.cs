using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class LisapalveluRaportointi : ContentPage
    {
        public LisapalveluRaportointi()
        {
            InitializeComponent();
        }

        private void LuoRaporttiButton_Clicked(object sender, EventArgs e)
        {
            // Alla esimerkki dataa testausta varten, tähän olisi tarkoitus tulla tietokannasta raporttitiedot 
            var tulokset = new List<string>
            {
                $"Raportti alueelta: {AluePicker.SelectedItem}",
                $"Aikaväliltä: {AloitusPvm.Date:d} - {LopetusPvm.Date:d}",
                "• Esimerkki-palvelu x 3",
                "• Siivous x 1",
                "• Aamiainen x 5"
            };

            RaporttiCollectionView.ItemsSource = tulokset;
        }
    }
}
