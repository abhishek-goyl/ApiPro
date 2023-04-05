using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.framework.net.Lib.Contracts
{
    public interface ISqlDataProvider
    {
        DataSet ExecuteProcedure(string connString, string query, SqlParameter[] parameters);
        DataSet ExecuteQuery(string connString, string query, SqlParameter[] parameters);
    }
}
