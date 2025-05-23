using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace TreeExplorer.Services
{
  public class DatabaseService
  {
    private string _connectionString;

    public void SetConnectionString(string server, string port, string database, string username, string password)
    {
      _connectionString = $"Host={server};Port={port};Database={database};Username={username};Password={password};";
    }

    public async Task<List<string>> GetTablesAsync()
    {
      var tables = new List<string>();

      using (var conn = new NpgsqlConnection(_connectionString))
      {
        await conn.OpenAsync();

        // Récupération des tables du schéma public
        using (var cmd = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.tables " +
            "WHERE table_schema = 'public' ORDER BY table_name", conn))
        {
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              tables.Add(reader.GetString(0));
            }
          }
        }
      }

      return tables;
    }

    public async Task<List<string>> GetViewsAsync()
    {
      var views = new List<string>();

      using (var conn = new NpgsqlConnection(_connectionString))
      {
        await conn.OpenAsync();

        // Récupération des vues du schéma public
        using (var cmd = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.views " +
            "WHERE table_schema = 'public' ORDER BY table_name", conn))
        {
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              views.Add(reader.GetString(0));
            }
          }
        }
      }

      return views;
    }
  }
}
