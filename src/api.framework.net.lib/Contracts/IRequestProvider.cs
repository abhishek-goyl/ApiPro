using api.framework.net.ExceptionLib;
using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace api.framework.net.Lib.Contracts
{
    public interface IRequestProvider
    { 
        object ResponseData { get; set; }
        int ResponseHttpStatusCode { get; set; }
        string GetCurrentController(HttpControllerContext context);
        string GetCurrentAction(HttpControllerContext context);
        string GetCurrentVersion(HttpControllerContext context);
        ApiEndpoint GetCurrentSchemaEndpoint(HttpControllerContext context, string httpMethod = null);
        List<ApiInput> getInputs(ApiEndpoint endpoint, HttpControllerContext context);
        List<ApiError> validateInputs(List<ApiInput> inputs);
        void ExecuteOperations(ApiEndpoint endpoint, List<ApiInput> inputs);
        void ManageResponseHeader(ApiEndpoint endpoint, HttpControllerContext context);
        string GetStatsData(string path);
        void CleanStatsData();
        void ValidateResponse();
        bool IsMockRequest(out string scenario);
    };
}
