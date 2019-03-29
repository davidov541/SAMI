using System;
using System.Data;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Database
{
    /// <summary>
    /// Interface for a component which accesses a SQL database.
    /// </summary>
    internal interface IDatabaseManager : IIOInterface
    {
        String ServerName
        {
            get;
        }

        /// <summary>
        /// The connection ID to the blob storage element.
        /// </summary>
        String BlobConnectionId
        {
            get;
        }

        /// <summary>
        /// Connects to the database and starts a session.
        /// </summary>
        void StartSession();

        /// <summary>
        /// Logs out of the database and ends the session.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Tries to run the given query, which returns no results. 
        /// </summary>
        /// <param name="query">Query to run.</param>
        /// <returns>True if the query was successful, false otherwise.</returns>
        bool TryRunNoResultQuery(String query);

        /// <summary>
        /// Tries to run the given query, returning any results.
        /// </summary>
        /// <param name="query">Query to run.</param>
        /// <param name="result">The result of the query, as an IDataReader.</param>
        /// <returns>True if the query was successful, false otherwise.</returns>
        bool TryRunResultQuery(String query, out IDataReader result);
    }
}
