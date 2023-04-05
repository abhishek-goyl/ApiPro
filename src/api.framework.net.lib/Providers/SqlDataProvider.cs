using api.logging;
using api.framework.net.Lib.Contracts;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;
using api.framework.net.ExceptionLib;

namespace api.framework.net.Lib.Providers
{
    public class SqlDataProvider : ISqlDataProvider
    {
        IConfiguration _configuration { get; set; }
        GlobalApplicationVariables globalvariable = new GlobalApplicationVariables();

        public SqlDataProvider(IConfiguration configProvider)
        {
            this._configuration = configProvider;
        }

        public DataSet ExecuteQuery(string connString, string query, SqlParameter[] parameters)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                return GetData(connString, query, CommandType.Text, parameters);
            }
            finally
            {
                log.Exit(true);
            }
        }

        public DataSet ExecuteProcedure(string connString, string query, SqlParameter[] parameters)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                return GetData(connString, query, CommandType.StoredProcedure, parameters);
            }
            finally
            {
                log.Exit(true);
            }
        }

        private DataSet GetData(string db, string comTxt, CommandType type, SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            LogEvent log = LogEvent.Start();
            StringBuilder pars = new StringBuilder();
            try
            {
                string connectionString = _configuration.GetValue<string>(db);
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    var sqlCommand = new SqlCommand(comTxt, sqlConnection);
                    SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                    sqlCommand.CommandType = type;
                    sqlCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SqlTimeout"] ?? Constants.DEFAULT_SQL_TIMEOUT);
                    if (parameters != null)
                    {
                        parameters.ToList().ForEach(p => sqlCommand.Parameters.AddWithValue(p.ParameterName, p.Value));
                        parameters.ToList().ForEach(p => pars.Append(string.Format("@{0}='{1}',", p.ParameterName, p.Value)));
                        log.Debug(pars.ToString());
                    }
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                AppException aEx = new AppException("Technical error occurred. Please contact system administrator.", "ServerError", ex);
                log.LogError(aEx);
                throw aEx;
            }
            finally
            {
                log.Exit();
            }
            return ds;
        }
    }
}
