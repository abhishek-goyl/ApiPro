using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using api.framework.net.ExceptionLib;

namespace api.framework.net.Lib.Providers
{
    public class MockSqlProvider : IMockSqlProvider
    {
        IConfiguration ConfigProvider { get; set; }

        public MockSqlProvider(IConfiguration _configProvider)
        {
            this.ConfigProvider = _configProvider;
        }

        /// <summary>
        /// Function to add a new mock data for the stored procedure
        /// </summary>
        /// <param name="name">string: name of the stored procedure</param>
        /// <param name="data">XmlDocument: mock data</param>
        /// <returns>ApiMockResponse: response</returns>
        public ApiMockResponse AddStoredProcedureMock(string name, string scenario, DataSet data)
        {
            LogEvent log = LogEvent.Start();
            ApiMockResponse res = new ApiMockResponse();
            try
            {
                var mockFile = GetSqlMockFileName(name, scenario);
                res.MockFile = mockFile;
                if (File.Exists(mockFile)) File.Delete(mockFile);
                File.WriteAllText(mockFile, JsonConvert.SerializeObject(data));
                res.Message = "success";
                res.Status = true;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                res.Error = ex.Message;
            }
            finally
            {
                log.Exit();
            }
            return res;
        }

        /// <summary>
        /// Function to delete the mock data for the stored procedure
        /// </summary>
        /// <param name="name">string: sp name</param>
        /// <returns>ApiMockResponse: response</returns>
        public ApiMockResponse DeleteStoredProcedureMock(string name, string scenario)
        {
            LogEvent log = LogEvent.Start();
            ApiMockResponse res = new ApiMockResponse();
            try
            {
                var mockFile = GetSqlMockFileName(name, scenario);
                res.MockFile = mockFile;
                if (File.Exists(mockFile)) File.Delete(mockFile);
                res.Message = "Deleted successfully";
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                res.Error = ex.Message;
            }
            finally
            {
                log.Exit();
            }
            return res;
        }

        /// <summary>
        /// Function to execute the mocked stored procedure
        /// </summary>
        /// <param name="query">string: name of the procedure</param>
        /// <param name="parameters">List<SqlParameter>: parameters</param>
        /// <returns>DataSet: the mockoutput data</returns>
        public DataSet ExecuteProcedure(string query, string scenario, List<SqlParameter> parameters)
        {
            DataSet data = new DataSet();
            LogEvent log = LogEvent.Start();
            try
            {
                var mockFile = GetSqlMockFileName(query, scenario);
                if (File.Exists(mockFile))
                {
                    var jsonData = File.ReadAllText(mockFile);
                    data = JsonConvert.DeserializeObject<DataSet>(jsonData);
                    if (HasPagingParameter(parameters))
                    {
                        int pageNo = GetPatameterValue<int>(parameters, "pageno");
                        int pageSize = GetPatameterValue<int>(parameters, "pagesize");
                        DataTable pgtable = new DataTable();
                        try
                        {
                            pgtable = data.Tables[0].AsEnumerable()
                                .Skip(pageSize * (pageNo - 1))
                                .Take(pageSize)
                                .CopyToDataTable();
                        }
                        catch { }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(pgtable);
                        for (int i = 1; i < data.Tables.Count; i++)
                        {
                            if (data.Tables[i] != null)
                            {
                                DataTable t = data.Tables[i].Rows.Count > 0 ? data.Tables[i].AsEnumerable().CopyToDataTable() : new DataTable();
                                ds.Tables.Add(t);
                            }
                        }
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return data;
        }
        

        #region Private Methods

        private string GetSqlMockFileName(string procedure, string scenario)
        {
            string filename = string.Empty;
            LogEvent log = LogEvent.Start();
            try
            {
                var basePath = ConfigProvider.GetValue<string>(Constants.MOCK_DATA_STORE);
                var path = Path.Combine("sql", string.IsNullOrEmpty(scenario) ? string.Format("{0}.json", procedure) : string.Format("{0}_{1}.json", procedure, scenario));
                filename = Path.Combine(basePath, path);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return filename;
        }


        private bool HasPagingParameter(List<SqlParameter> parameters)
        {
            bool flag = false;
            LogEvent log = LogEvent.Start();
            try
            {
                if (parameters.Where(p => p.ParameterName.ToLower().Equals("pageno")).Count() > 0
                    & parameters.Where(p => p.ParameterName.ToLower().Equals("pagesize")).Count() > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return flag;
        }

        T GetPatameterValue<T>(List<SqlParameter> parameters, string name)
        {
            T val = default(T);
            LogEvent log = LogEvent.Start();
            try
            {
                if (parameters.Where(p => p.ParameterName.ToLower().Equals(name)).Count() > 0)
                {
                    var tmp = parameters.Find(p => p.ParameterName.ToLower().Equals(name)).Value.ToString();
                    val = JsonConvert.DeserializeObject<T>(tmp);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw new AppException(ex.Message);
            }
            finally
            {
                log.Exit();
            }
            return val;
        }

        #endregion
    }
}
