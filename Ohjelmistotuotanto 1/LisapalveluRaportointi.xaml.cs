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
            // tähän olisi tarkoitus tulla tietokannasta raporttitiedot 
            var tulokset = new List<string>
            {
               
            };

            RaporttiCollectionView.ItemsSource = tulokset;
        }
    }
}
