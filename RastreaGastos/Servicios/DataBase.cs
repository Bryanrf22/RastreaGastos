using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RastreaGastos.Modelos;

namespace RastreaGastos.Servicios
{
    public class DataBase
    {
        private readonly string _databasePath;
        private readonly string _connectioString;
        private Task? _iniciarTask;

        public DataBase()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "gastos.db3");
            _connectioString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath
            }.ToString();
        }

        //crear tabla en caso de que no exista
        public Task IniciarBDAsync()
        {
            return _iniciarTask ??= CrearBDAsync();
        }

        private Task CrearBDAsync()
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                @"CREATE TABLE IF NOT EXISTS Gastos (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                    Descripcion TEXT NOT NULL ,
                    Monto REAL NOT NULL ,
                    Fecha TEXT DEFAULT CURRENT_TIMESTAMP
                );";
                tableCommand.ExecuteNonQuery();
            });
        }

        public async Task<List<Gastos>> LeerGastosAsync(string? filtro)
        {
            await IniciarBDAsync();

            return await Task.Run(() =>
            {
                var gastos = new List<Gastos>();
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = @"SELECT ID, Descripcion, Monto, Fecha FROM Gastos";
                using var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    gastos.Add(new Gastos
                    {
                        ID = reader.GetInt32(0),
                        Descripcion = reader.GetString(1),
                        Monto = reader.GetDecimal(2),
                        Fecha = reader.GetDateTime(3)
                    });
                }
                return gastos;
            });
        }

        public async Task<int> AgregarGastoAsync(Gastos gastos)
        {
            await IniciarBDAsync();

            return await Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText =
                @"INSERT INTO Gastos (Descripcion, Monto) VALUES (@Descripcion, @Monto);";
                insertCommand.Parameters.AddWithValue("@Descripcion", gastos.Descripcion);
                insertCommand.Parameters.AddWithValue("@Monto", gastos.Monto);
                return insertCommand.ExecuteNonQuery();
            });
        }

        public async Task<int> ActualizarGastoAsync(Gastos gastos)
        {
            await IniciarBDAsync();

            return await Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText =
                @"UPDATE Gastos SET Descripcion = @Descripcion, Monto = @Monto WHERE ID = @ID;";
                updateCommand.Parameters.AddWithValue("@Descripcion", gastos.Descripcion);
                updateCommand.Parameters.AddWithValue("@Monto", gastos.Monto);
                updateCommand.Parameters.AddWithValue("@ID", gastos.ID);
                return updateCommand.ExecuteNonQuery();
            });
        }

        public async Task<int> EliminarGastoAsync(int id)
        {
            await IniciarBDAsync();

            return await Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText =
                @"DELETE FROM Gastos WHERE ID = @ID;";
                deleteCommand.Parameters.AddWithValue("@ID", id);
                return deleteCommand.ExecuteNonQuery();
            });
        }
    }
}
