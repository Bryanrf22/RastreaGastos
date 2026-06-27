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

        public DataBase()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "DB_Gastos.db3");
            _connectioString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath
            }.ToString();
        }

        //crear tabla en caso de que no exista
        public Task IniciarBDAsync()
        {
            return Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                @"CREATE TABLE IF NOT EXISTS Gastos (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Descripcion VARCHAR(200) NOT NULL,
                    Monto REAL NOT NULL,
                    Fecha DATE DEFAULT GETDATE()
                );";
                tableCommand.ExecuteNonQuery();
            });
        }

        public Task<List<Gastos>> LeerGastosAsync(string filtro)
        {
            return Task.Run(() =>
            {
                var gastos = new List<Gastos>();
                using var connection = new SqliteConnection(_connectioString);
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = @"SELECT * FROM Gastos";
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

        public Task<int> AgregarGastoAsync(Gastos gastos)
        {
            return Task.Run(() =>
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

        public Task<int> ActualizarGastoAsync(Gastos gastos)
        {
            return Task.Run(() =>
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

        public Task<int> EliminarGastoAsync(int id)
        {
            return Task.Run(() =>
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
