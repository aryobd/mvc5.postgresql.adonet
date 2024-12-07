using Npgsql;
using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Eksekusi INSERT, UPDATE, DELETE
        public void ExecuteNonQuery(string sql, Action<NpgsqlCommand> configureParams)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    configureParams?.Invoke(cmd);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        // Eksekusi SELECT Data
        public List<T> ExecuteQuery<T>(string sql, Action<NpgsqlCommand> configureParams, Func<IDataReader, T> map)
        {
            var result = new List<T>();

            using (var conn = new NpgsqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                configureParams?.Invoke(cmd);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(map(reader));
                    }
                }
            }
            return result;
        }

        // Tambahkan Parameter
        public void AddParameter(NpgsqlCommand cmd, string paramName, NpgsqlTypes.NpgsqlDbType dbType, object value)
        {
            cmd.Parameters.Add(new NpgsqlParameter(paramName, dbType) { Value = value ?? DBNull.Value });
        }
    }
}
