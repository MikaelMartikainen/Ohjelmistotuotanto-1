using Microsoft.Maui.Controls;

namespace Ohjelmistotuotanto_1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Register routes for navigation
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(Aluehallintav2), typeof(Aluehallintav2));
            Routing.RegisterRoute(nameof(PalveluidenHallinta), typeof(PalveluidenHallinta));
            Routing.RegisterRoute(nameof(MokkiHallinta), typeof(MokkiHallinta));
            Routing.RegisterRoute(nameof(AsiakasHallinta), typeof(AsiakasHallinta));
            Routing.RegisterRoute(nameof(Varaushallinta), typeof(Varaushallinta));
        }
    }
}
