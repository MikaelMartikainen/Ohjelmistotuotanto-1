using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Ohjelmistotuotanto_1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<DatabaseHelper>();
            
            // Register pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Aluehallintav2>();
            builder.Services.AddTransient<PalveluidenHallinta>();
            builder.Services.AddTransient<MokkiHallinta>();
            builder.Services.AddTransient<AsiakasHallinta>();
            builder.Services.AddTransient<Varaushallinta>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
