using api.framework.net.Lib.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace api.framework.net.Lib.Contracts
{
    public interface IMockSqlProvider
    {
        /// <summary>
        /// Function to add a new mock data for the stored procedure
        /// </summary>
        /// <param name="name">string: name of the stored procedure</param>
        /// <param name="data">DataSet: mock data</param>
        ApiMockResponse AddStoredProcedureMock(string name, string scenario, DataSet data);


        /// <summary>
        /// Function to delete the mock data for the stored procedure
        /// </summary>
        /// <param name="name"></param>
        ApiMockResponse DeleteStoredProcedureMock(string name, string scenario);

        /// <summary>
        /// Function to execute the mocked stored procedure
        /// </summary>
        /// <param name="query">string: name of the procedure</param>
        /// <param name="parameters">List<SqlParameter>: parameters</param>
        /// <returns>DataSet: the mockoutput data</returns>
        DataSet ExecuteProcedure(string query, string scenario, List<SqlParameter> parameters);
    }
}
