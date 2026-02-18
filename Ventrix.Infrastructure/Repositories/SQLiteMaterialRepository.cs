using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Entities;
using System.Data.SQLite;

namespace Ventrix.Infrastructure.Repositories
{
    public class SQLiteMaterialRepository : IMaterialRepository
    {
        private readonly string _connectionString = "Data Source=ventrix.db;Version=3;";

        public IEnumerable<Material> GetAll()
        {
            var list = new List<Material>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Inventory", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Material
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Status = reader["Status"].ToString(),
                            Condition = reader["Condition"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public IEnumerable<Material> GetByStatus(string status)
        {
            // Implementation uses the Domain entity to filter data
            return GetAll().Where(m => m.Status == status);
        }

        public void UpdateStatus(string id, string newStatus)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Inventory SET Status = @status WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}