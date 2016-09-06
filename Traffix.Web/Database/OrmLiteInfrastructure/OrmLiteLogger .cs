using System.Data;
using System.Data.Common;

namespace Traffix.Web.Database.OrmLiteInfrastructure
{

    public class OrmLiteConnection : IDbConnection
    {
        private readonly DbConnection _connection;

        public OrmLiteConnection(DbConnection connection)
        {
            _connection = connection;
        }

        protected DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            System.Diagnostics.Debug.WriteLine("Starting transaction");
            return _connection.BeginTransaction(isolationLevel);
        }

        public IDbTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _connection.BeginTransaction(isolationLevel);
        }

        public void Close()
        {
            _connection.Close();
        }

        public void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public IDbCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        public void Open()
        {
            _connection.Open();
        }

        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public int ConnectionTimeout { get; }

        public string Database => _connection.Database;

        public ConnectionState State => _connection.State;

        public string DataSource => _connection.DataSource;

        public string ServerVersion => _connection.ServerVersion;

        protected DbCommand CreateDbCommand()
        {
            var output = _connection.CreateCommand();
            System.Diagnostics.Debug.WriteLine(output.CommandText);
            return output;
        }

        public void Dispose()
        {
            if (this.State != ConnectionState.Closed)
            {
                this.Close();
            }
        }
    }
}
