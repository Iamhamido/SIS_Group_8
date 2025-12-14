// DatabaseContext.cs
using MySql.Data.MySqlClient;
using System;

namespace SIS.Data
{
    public class DatabaseContext : IDisposable
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
            }
            
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
            
            return _connection;
        }

        public void ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new MySqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
        }

        public MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] parameters)
        {
            var connection = GetConnection();
            var command = new MySqlCommand(sql, connection);
            
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            
            return command.ExecuteReader();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}