using Microsoft.Extensions.DependencyInjection;
using RastreaGastos.Servicios;
using SQLitePCL;

namespace RastreaGastos
{
    public partial class App : Application
    {
        public App(DataBase db)
        {
            InitializeComponent();
            Batteries_V2.Init();
            _ = db.IniciarBDAsync();

            MainPage = new AppShell();
        }
    }
}
