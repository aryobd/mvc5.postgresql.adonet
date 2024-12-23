﻿using Npgsql;
using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DatabaseService
    {
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["pdam_Entities"].ConnectionString;

        public DatabaseService()
        {
        }

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // TAMBAHKAN PARAMETER
        public void AddParameter(NpgsqlCommand cmd, string paramName, NpgsqlTypes.NpgsqlDbType dbType, object value)
        {
            cmd.Parameters.Add(new NpgsqlParameter(paramName, dbType) { Value = value ?? DBNull.Value });
        }

        // EKSEKUSI "SELECT" DATA
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

        // EKSEKUSI "INSERT"/"UPDATE"/"DELETE"
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

                        throw;
                    }
                }
            }
        }

        // EKSEKUSI "INSERT"/"UPDATE"/"DELETE" - MENGEMBALIKAN JUMLAH BARIS YANG TERDAMPAK
        public int ExecuteNonQuery2(string sql, Action<NpgsqlCommand> configureParams)
        {
            int rowsAffected = 0;

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
                        rowsAffected = cmd.ExecuteNonQuery();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        throw;
                    }
                    finally
                    {
                    }
                }
            }

            return rowsAffected;
        }

        // EKSEKUSI "INSERT", "UPDATE", "DELETE" -- ATOMIC
        public void ExecuteTransaction(Action<NpgsqlCommand> executeCommands)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.Transaction = trans;

                    try
                    {
                        executeCommands?.Invoke(cmd);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        throw;
                    }
                }
            }
        }

        // EKSEKUSI "INSERT", UPDATE", "DELETE" -- ATOMIC - MEMASTIKAN SETIAP OPERASI ADA BARIS DATA YANG TERDAMPAK
        public void ExecuteTransaction(List<(string query, Action<NpgsqlCommand> configureParams)> actions)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var trans = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.Transaction = trans;

                    try
                    {
                        foreach (var action in actions)
                        {
                            cmd.CommandText = action.query;

                            action.configureParams?.Invoke(cmd);

                            int rowsAffected = cmd.ExecuteNonQuery(); // JUMLAH BARIS DATA YANG TERDAMPAK

                            if (rowsAffected <= 0)
                                throw new Exception($"Execution failed for query: {action.query}");
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        throw;
                    }
                }
            }
        }
    }
}
