using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Database
{
    [ParseableElement("Database", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class DatabaseManager : IDatabaseManager
    {
        private SqlConnection _connection;
        private static EventWaitHandle _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        private String _portNumber, _userName, _password;

        /// <inheritdoc />
        public String Name
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public String ServerName
        {
            get;
            private set;
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("ServerName", () => ServerName, name => ServerName = name);
                yield return new PersistentProperty("Port", () => _portNumber, port => _portNumber = port);
                yield return new PersistentProperty("Username", () => _userName, username => _userName = username);
                yield return new PersistentProperty("Password", () => _password, pass => _password = pass);
                yield return new PersistentProperty("BlobConnectionString", () => BlobConnectionId, id => BlobConnectionId = id);
                yield return new PersistentProperty("Name", () => Name, id => Name = id);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public String BlobConnectionId
        {
            get;
            private set;
        }

        public void Initialize(IConfigurationManager configManager)
        {
        }

        public void Dispose()
        {
        }

        public void StartSession()
        {
            _waitHandle.WaitOne();
            _waitHandle.Reset();

            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = String.Format("tcp:{0},{1}", ServerName, _portNumber);
            connectionString.ConnectTimeout = 30;
            connectionString.Encrypt = true;
            connectionString.UserID = _userName;
            connectionString.Password = _password;
            connectionString.InitialCatalog = "SAMI";
            connectionString.Pooling = true;
            connectionString.ApplicationIntent = ApplicationIntent.ReadWrite;
            connectionString.AsynchronousProcessing = false;
            connectionString.IntegratedSecurity = false;

            _connection = new SqlConnection(connectionString.ConnectionString);
            _connection.Open();
        }

        public void EndSession()
        {
            TryRunNoResultQuery(String.Format("DELETE FROM [dbo].[Logs] WHERE Time < '{0}'", DateTime.Now.Subtract(new TimeSpan(15, 0, 0, 0)).ToString()));
            _connection.Close();
            _waitHandle.Set();
        }

        public void AddChild(IParseable component)
        {
        }

        public bool TryRunNoResultQuery(String query)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, _connection);
                int numberOfRows = command.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        public bool TryRunResultQuery(String query, out IDataReader result)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, _connection);
                result = command.ExecuteReader();
            }
            catch (SqlException)
            {
                result = null;
                return false;
            }
            catch (InvalidOperationException)
            {
                result = null;
                return false;
            }
            return true;
        }
    }
}
