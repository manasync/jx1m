global using MySQLException = MySqlConnector.MySqlException;
global using MySQLParameter = MySqlConnector.MySqlParameter;

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using MySqlConnector;

namespace MySQLDriverCS
{
    public static class TianMaCharSet
    {
        public static int ConvertToCodePage { get; set; } = 0;
    }

    public class MySQLConnectionString
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public uint Port { get; set; } = 3306;
        public string AsString { get; private set; }

        public MySQLConnectionString(string host, string database, string user, string password, uint port = 3306)
        {
            Host = host;
            Database = database;
            User = user;
            Password = password;
            Port = port;

            var builder = new MySqlConnectionStringBuilder
            {
                Server = host,
                Database = database,
                UserID = user,
                Password = password,
                Port = port,
                CharacterSet = "utf8mb4",
                AllowUserVariables = true,
                DefaultCommandTimeout = 30,
                Pooling = true,
                MinimumPoolSize = 5,
                MaximumPoolSize = 100
            };
            AsString = builder.ConnectionString;
        }
    }

    public class MySQLConnection : IDisposable
    {
        private readonly MySqlConnection _innerConn;

        public MySQLConnection(string connectionString)
        {
            _innerConn = new MySqlConnection(connectionString);
        }

        public MySqlConnection InnerConnection => _innerConn;

        public ConnectionState State => _innerConn.State;

        public void Open()
        {
            if (_innerConn.State != ConnectionState.Open)
            {
                _innerConn.Open();
            }
        }

        public void Close()
        {
            _innerConn.Close();
        }

        public void Dispose()
        {
            _innerConn.Dispose();
        }

        public MySQLDataReader ExecuteReader(string sql)
        {
            using var cmd = new MySQLCommand(sql, this);
            return cmd.ExecuteReader();
        }

        public MySQLCommand CreateCommand()
        {
            return new MySQLCommand(string.Empty, this);
        }
    }

    public class MySQLCommand : IDisposable
    {
        private readonly MySqlCommand _innerCmd;
        private MySQLConnection _conn;

        public MySQLCommand(string commandText, MySQLConnection connection)
        {
            _innerCmd = new MySqlCommand(commandText, connection?.InnerConnection);
            _conn = connection;
        }

        public MySqlCommand InnerCommand => _innerCmd;

        public MySQLConnection Connection
        {
            get => _conn;
            set
            {
                _conn = value;
                _innerCmd.Connection = value?.InnerConnection;
            }
        }

        public string CommandText
        {
            get => _innerCmd.CommandText;
            set => _innerCmd.CommandText = value;
        }

        public int CommandTimeout
        {
            get => _innerCmd.CommandTimeout;
            set => _innerCmd.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => _innerCmd.CommandType;
            set => _innerCmd.CommandType = value;
        }

        public MySQLTransaction Transaction
        {
            set => _innerCmd.Transaction = value?.InnerTransaction;
        }

        public MySqlParameterCollection Parameters => _innerCmd.Parameters;

        public int ExecuteNonQuery() => _innerCmd.ExecuteNonQuery();

        public object ExecuteScalar() => _innerCmd.ExecuteScalar();

        public MySQLDataReader ExecuteReader()
        {
            return new MySQLDataReader(_innerCmd.ExecuteReader());
        }

        public MySQLDataReader ExecuteReaderEx()
        {
            return ExecuteReader();
        }

        public void Dispose()
        {
            _innerCmd.Dispose();
        }
    }

    public class MySQLDataReader : DbDataReader
    {
        private readonly MySqlDataReader _inner;

        public MySQLDataReader(MySqlDataReader inner)
        {
            _inner = inner;
        }

        public MySqlDataReader InnerReader => _inner;

        public override int FieldCount => _inner.FieldCount;
        public override bool HasRows => _inner.HasRows;
        public override bool IsClosed => _inner.IsClosed;
        public override int RecordsAffected => _inner.RecordsAffected;
        public override int Depth => _inner.Depth;

        public override object this[int ordinal] => _inner[ordinal];
        public override object this[string name] => _inner[name];

        public override bool Read() => _inner.Read();
        public override bool NextResult() => _inner.NextResult();
        public override void Close() => _inner.Close();

        public override bool IsDBNull(int ordinal) => _inner.IsDBNull(ordinal);
        public override bool GetBoolean(int ordinal) => _inner.GetBoolean(ordinal);
        public override byte GetByte(int ordinal) => _inner.GetByte(ordinal);
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => _inner.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public override char GetChar(int ordinal) => _inner.GetChar(ordinal);
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            => _inner.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public override string GetDataTypeName(int ordinal) => _inner.GetDataTypeName(ordinal);
        public override DateTime GetDateTime(int ordinal) => _inner.GetDateTime(ordinal);
        public override decimal GetDecimal(int ordinal) => _inner.GetDecimal(ordinal);
        public override double GetDouble(int ordinal) => _inner.GetDouble(ordinal);
        public override Type GetFieldType(int ordinal) => _inner.GetFieldType(ordinal);
        public override float GetFloat(int ordinal) => _inner.GetFloat(ordinal);
        public override Guid GetGuid(int ordinal) => _inner.GetGuid(ordinal);
        public override short GetInt16(int ordinal) => _inner.GetInt16(ordinal);
        public override int GetInt32(int ordinal) => _inner.GetInt32(ordinal);
        public override long GetInt64(int ordinal) => _inner.GetInt64(ordinal);
        public override string GetName(int ordinal) => _inner.GetName(ordinal);
        public override int GetOrdinal(string name) => _inner.GetOrdinal(name);
        public override string GetString(int ordinal) => _inner.IsDBNull(ordinal) ? string.Empty : _inner.GetString(ordinal);
        public override object GetValue(int ordinal) => _inner.GetValue(ordinal);
        public override int GetValues(object[] values) => _inner.GetValues(values);
        public override IEnumerator GetEnumerator() => _inner.GetEnumerator();
        public override DataTable GetSchemaTable() => _inner.GetSchemaTable();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class MySQLDataAdapter : IDisposable
    {
        private readonly MySqlDataAdapter _inner;

        public MySQLDataAdapter(MySQLCommand cmd)
        {
            _inner = new MySqlDataAdapter(cmd?.InnerCommand);
        }

        public int Fill(DataTable dataTable) => _inner.Fill(dataTable);
        public int Fill(DataSet dataSet) => _inner.Fill(dataSet);

        public void Dispose() => _inner.Dispose();
    }

    public class MySQLTransaction : IDisposable
    {
        private readonly MySqlTransaction _innerTrans;
        public MySQLTransaction(MySqlTransaction innerTrans)
        {
            _innerTrans = innerTrans;
        }

        public MySqlTransaction InnerTransaction => _innerTrans;

        public void Commit() => _innerTrans.Commit();
        public void Rollback() => _innerTrans.Rollback();
        public void Dispose() => _innerTrans.Dispose();
    }

    public class MySQLSelectCommand
    {
        public DataTable Table { get; } = new DataTable();

        public MySQLSelectCommand(
            MySQLConnection conn,
            string[] fields,
            string[] tables,
            object[,] where = null,
            string[] groupBy = null,
            string[,] orderBy = null,
            bool distinct = false,
            int offset = 0,
            int limit = 0,
            bool forUpdate = false)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (distinct) sb.Append("DISTINCT ");
            sb.Append(fields != null && fields.Length > 0 ? string.Join(", ", fields) : "*");
            sb.Append(" FROM ");
            sb.Append(string.Join(", ", tables));

            var cmd = conn.InnerConnection.CreateCommand();

            if (where != null && where.GetLength(0) > 0)
            {
                sb.Append(" WHERE ");
                for (int i = 0; i < where.GetLength(0); i++)
                {
                    if (i > 0) sb.Append(" AND ");
                    string col = where[i, 0]?.ToString();
                    string op = where[i, 1]?.ToString();
                    object val = where[i, 2];

                    string paramName = $"@p_where_{i}";
                    sb.Append($"`{col}` {op} {paramName}");
                    cmd.Parameters.AddWithValue(paramName, val ?? DBNull.Value);
                }
            }

            if (groupBy != null && groupBy.Length > 0)
            {
                sb.Append(" GROUP BY ");
                sb.Append(string.Join(", ", groupBy));
            }

            if (orderBy != null && orderBy.GetLength(0) > 0)
            {
                sb.Append(" ORDER BY ");
                for (int i = 0; i < orderBy.GetLength(0); i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append($"`{orderBy[i, 0]}` {orderBy[i, 1]}");
                }
            }

            if (limit > 0)
            {
                if (offset > 0)
                    sb.Append($" LIMIT {offset}, {limit}");
                else
                    sb.Append($" LIMIT {limit}");
            }

            if (forUpdate)
            {
                sb.Append(" FOR UPDATE");
            }

            cmd.CommandText = sb.ToString();
            using var reader = cmd.ExecuteReader();
            Table.Load(reader);
        }

        public MySQLSelectCommand(
            MySQLConnection conn,
            string[] fields,
            string[] tables,
            object[,] where,
            string[] groupBy,
            string[,] orderBy) : this(conn, fields, tables, where, groupBy, orderBy, false, 0, 0, false)
        {
        }
    }
}
