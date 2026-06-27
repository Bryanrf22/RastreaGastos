using Microsoft.Maui.Controls;
using RastreaGastos.Modelos;
using RastreaGastos.Servicios;
using System.Threading.Tasks;

namespace RastreaGastos;

public partial class GastoPage : ContentPage
{
	private readonly Gastos? _gasto;
	private readonly DataBase _dbService;
	public GastoPage(DataBase dbService, Gastos? gasto = null)
	{
		InitializeComponent();
		_dbService = dbService;
		_gasto = gasto;

		if (_gasto != null)
		{
			entryDesc.Text = _gasto.Descripcion;
			entryMonto.Text = _gasto.Monto.ToString();
		}
	}

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
		if (string.IsNullOrWhiteSpace(entryDesc.Text) || string.IsNullOrWhiteSpace(entryMonto.Text) 
			|| !decimal.TryParse(entryMonto.Text, out decimal monto))
		{
			await DisplayAlert("Error", "Por favor, llene todos los campos con informacion valida.", "Aceptar");
			return;
		}
		_gasto!.Descripcion = entryDesc.Text;
		_gasto!.Monto = monto;
		if (_gasto.ID == 0)
		{
			await _dbService.AgregarGastoAsync(_gasto);
		}
		else
		{
			await _dbService.ActualizarGastoAsync(_gasto);
		}
		await Navigation.PopAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}
