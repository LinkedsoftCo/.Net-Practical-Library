using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extentions;

namespace DatabaseCommunications
{
    #region ADO.NET DATA ACCESS CLASS -------------------------------------------------------------------------------------
    public class DbCore
    {
        #region DIM-----------------------------------------------------------------------------------------------------
        private static readonly DbProviderFactory Factory =
            DbProviderFactories.GetFactory(ConfigurationManager.AppSettings.Get("DataProvider"));  // System.Data.SqlClient


        #endregion

        #region PROPERTY -----------------------------------------------------------------------------------------------
        private string ConnectionString { get; set; }
        #endregion

        #region METHOD -------------------------------------------------------------------------------------------------
        public DbCore(string type)
        {
            //var connector = ConnectionFactories.GetConnection(type ?? "UM");
            //ConnectionString = connector.GetConnectionString();
        }

        /// <summary>
        /// If connection is open Return true else return false (the connection is not open).
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean CheckConnection()
        {
            var connection = Factory.CreateConnection();
            if (string.IsNullOrEmpty(ConnectionString)) return false;
            if (connection == null) return true;
            connection.ConnectionString = ConnectionString;
            connection.Open();
            connection.Close();
            return true;
        }

        #endregion

        #region PRIMARY METHODS ----------------------------------------------------------------------------------------

        DbConnection CreateConnection()
        {
            var connection = Factory.CreateConnection();
            if (string.IsNullOrEmpty(ConnectionString)) return null;
            if (connection == null) return null;
            connection.ConnectionString = ConnectionString;
            connection.Open();

            return connection;
        }

        private static DbCommand CreateCommand(string sql, DbConnection conn, params object[] parms)
        {
            var command = Factory.CreateCommand();
            if (command == null) return null;
            command.Connection = conn;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            DbExtentions.AddParameters(command, parms);
            return command;
        }

        private static DbCommand CreateStoredProcedureCommand(string storedProcedureName, DbConnection conn,
            params object[] parms)
        {
            var command = Factory.CreateCommand();
            if (command == null) return null;
            command.Connection = conn;
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;
            DbExtentions.AddParameters(command, parms);

            return command;
        }

        private static DbDataAdapter CreateAdapter(DbCommand command)
        {
            var adapter = Factory.CreateDataAdapter();
            if (adapter == null) return null;
            adapter.SelectCommand = command;

            return adapter;
        }

        #region T-SQL Command

        /// <summary>
        /// fast read and instantiate a collection of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="make"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IEnumerable<T> Read<T>(string sql, Func<IDataReader, T> make, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateCommand(sql, connection, parms))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return make(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adapter data reader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public DataSet Read(string sql, params object[] parms)
        {
            var dataset = new DataSet();
            using (var connection = CreateConnection())
            {
                using (var da = CreateAdapter(CreateCommand(sql, connection, parms)))
                {
                    da.Fill(dataset);
                }
            }

            return dataset;
        }

        /// <summary>
        /// Insert a new record
        /// </summary>
        /// <param name="sql">T-SQL string</param>
        /// <param name="parms">object array of parameters</param>
        /// <returns></returns>
        public int Insert(string sql, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateCommand(sql + ";SELECT SCOPE_IDENTITY();", connection, parms))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        /// <summary>
        /// Update an existing record
        /// </summary>
        /// <param name="sql">T-SQL string</param>
        /// <param name="parms">object array of parameters</param>
        /// <returns></returns>
        public int Update(string sql, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateCommand(sql, connection, parms))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Delete a record
        /// </summary>
        /// <param name="sql">T-SQL string</param>
        /// <param name="parms">object array of stored procedure parameters</param>
        /// <returns></returns>
        public int Delete(string sql, params object[] parms)
        {
            return Update(sql, parms);
        }

        #endregion

        #region StoredProcedure Command

        /// <summary>
        /// fast read and instantiate a collection of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="make"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IEnumerable<T> SpRead<T>(string storedProcedureName, Func<IDataReader, T> make, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateStoredProcedureCommand(storedProcedureName, connection, parms))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return make(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// read data
        /// </summary>
        /// <param name="storedProcedureName">StoredProcedure name</param>
        /// <param name="parms">object array of stored procedure parameters</param>
        /// <returns></returns>
        public object SpSelect(string storedProcedureName, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateStoredProcedureCommand(storedProcedureName, connection, parms))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Insert a new record with any DBMS stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">stored procedure name</param>
        /// <param name="parms">object array of stored procedure parameters</param>
        /// <returns></returns>
        public int SpInsert(string storedProcedureName, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateStoredProcedureCommand(storedProcedureName, connection, parms))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        /// <summary>
        /// Update an existing record with any DBMS stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">stored procedure name</param>
        /// <param name="parms">object array of stored procedure parameters</param>
        /// <returns></returns>
        public int SpUpdate(string storedProcedureName, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateStoredProcedureCommand(storedProcedureName, connection, parms))
                {
                    return command.ExecuteNonQuery();
                }
            }

        }

        /// <summary>
        /// Delete a record with any DBMS stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">stored procedure name</param>
        /// <param name="parms">object array of stored procedure parameters</param>
        /// <returns></returns>
        public int SpDelete(string storedProcedureName, params object[] parms)
        {
            return SpUpdate(storedProcedureName, parms);
        }

        #endregion

        #endregion
    }
    #endregion
}
