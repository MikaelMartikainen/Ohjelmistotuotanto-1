namespace Ohjelmistotuotanto_1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //  pääsivuksi
            //return new Window(new MainPage());
            // Aluehallinta
            //return new Window(new Aluehallinta());
            //aluehallinta v2
            return new Window(new Aluehallintav2());

        }
    }
}