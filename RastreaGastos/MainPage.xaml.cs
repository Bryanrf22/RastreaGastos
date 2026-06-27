using RastreaGastos.Modelos;
using RastreaGastos.Servicios;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RastreaGastos
{
    public partial class MainPage : ContentPage
    {
        private readonly DataBase _db;
        private readonly ObservableCollection<Gastos> _gasto = [];

        public MainPage(DataBase db)
        {
            InitializeComponent();
            _db = db;
            ListaGastos.ItemsSource = _gasto;
            _ = Task.Run(async () => await CargarGastosAsync());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarGastosAsync(Busqueda.Text);
        }

        public async Task CargarGastosAsync(string? filtro = null)
        {
            _gasto.Clear();
            var gastos = await _db.LeerGastosAsync(filtro);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLowerInvariant();
                gastos = gastos.Where(g => g.Descripcion.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _gasto.Clear();
                foreach (var gst in gastos)
                {
                    gastos.Add(gst);
                }
            });
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
        }

        private async void OnEliminarGasto(object sender, EventArgs e)
        {
            if (sender is Button btn&&btn.BindingContext is Gastos gst)
            {
                var confirm = await DisplayAlert("Confirmar", $"¿Desea eliminar el gasto {gst.Descripcion} permanentemente? esta accion no se puede revertir", "Si", "No");
                if (!confirm)
                {
                    return;
                }
                await _db.EliminarGastoAsync(gst.ID);
                await CargarGastosAsync(Busqueda.Text);
            }
        }

        private void OnEditarGasto(object sender, EventArgs e)
        {

        }

        private void OnAgregarGasto(object sender, EventArgs e)
        {

        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            await CargarGastosAsync(e.NewTextValue);
        }
    }

}
