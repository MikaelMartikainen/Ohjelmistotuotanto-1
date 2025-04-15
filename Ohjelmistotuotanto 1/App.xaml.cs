namespace Ohjelmistotuotanto_1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
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