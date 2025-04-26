using Microsoft.Maui.Controls;
using System;

namespace Ohjelmistotuotanto_1
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();
                MainPage = new NavigationPage(new MainPage());
                
                // Add global unhandled exception handler
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }
            catch (Exception ex)
            {
                // Log initialization error
                System.Diagnostics.Debug.WriteLine($"App initialization error: {ex.Message}");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Unobserved Task Exception: {e.Exception.Message}");
            e.SetObserved(); // Mark as observed to prevent app crash
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"Unhandled Exception: {exception?.Message ?? "Unknown error"}");
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

       /* protected override Window CreateWindow(IActivationState? activationState)
        {
            //  pääsivuksi
            return new Window(new MainPage());
          
            
            //aluehallinta v2
           // return new Window(new Aluehallintav2());
            

        

        }*/
    }
}