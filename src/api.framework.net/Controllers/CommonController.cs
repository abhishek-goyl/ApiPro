using api.framework.net.Lib.Contracts;
using System.Collections.Generic;
using System.Web.Http;
using api.framework.net.Lib.Models;
using Newtonsoft.Json.Linq;
using System;
using api.framework.net.Lib.Providers;
using api.framework.net.Filters;
using api.logging;
using api.framework.net.ExceptionLib;

namespace api.framework.net.Controllers
{
    public class CommonController : ApiController
    {
        public IRequestProvider _requestHandler;
        public IApiSchemaProvider _schemaProvider;

        public CommonController(IConfiguration configProvider, ISqlDataProvider sqlDataProvider)
        {
            _requestHandler = new RequestProvider(sqlDataProvider, configProvider);
            _schemaProvider = new ApiSchemaProvider(configProvider);
        }

        public CommonController(IRequestProvider requestProvider, IApiSchemaProvider schemaProvider)
        {
            _requestHandler = requestProvider;
            _schemaProvider = schemaProvider;
        }

        [HttpGet]
        [ApiAuthorization]
        [ApiResponseStatus]
        [ApiExceptionFilter]
        public object Index()
        {
            return Get();
        }

        [HttpGet]
        [ApiResponseStatus]
        [ApiExceptionFilter]
        public object IndexPublic()
        {
            return Get();
        }

        [HttpPost]
        [ApiAuthorization]
        [ApiResponseStatus]
        [ApiExceptionFilter]
        public object Index(string data = "")
        {
            return Post();
        }

        [HttpPost]
        public object UploadFile()
        {
            return "Ok";
        }

        [HttpPost]
        [ApiResponseStatus]
        [ApiExceptionFilter]
        public object IndexPublic(object data)
        {
            return Post();
        }

        private object Get()
        {
            JObject response = new JObject();
            LogEvent log = LogEvent.Start();
            var errors = new List<ApiError>();
            try
            {
                var selected = _requestHandler.GetCurrentSchemaEndpoint(ControllerContext);
                var inputs = _requestHandler.getInputs(selected, ControllerContext);

                //validate inputs
                errors = _requestHandler.validateInputs(inputs);
                if (errors.Count == 0)
                {
                    _requestHandler.ExecuteOperations(selected, inputs);
                    _requestHandler.ManageResponseHeader(selected, ControllerContext);
                    _requestHandler.CleanStatsData();
                    _requestHandler.ValidateResponse();
                    // Handle DB Errors
                    if (_requestHandler.ResponseHttpStatusCode == 500)
                    {
                        throw new AppException(_requestHandler.ResponseData);
                    }
                }
                else
                {
                    // send 400 with respective errors
                    ErrorResponse res = new ErrorResponse { errors = new List<ApiError>() };
                    errors.RemoveAll(e => string.IsNullOrEmpty(e.errorMessage));
                    if (errors.Count > 0)
                    {
                        errors.ForEach(e => res.errors.Add(new ApiError { errorMessage = e?.errorMessage, errorCode = e?.errorCode }));
                        throw new BadRequestException(res);
                    }
                    _requestHandler.ValidateResponse();
                }
                return _requestHandler.ResponseData;
            }
            catch (AppException ex)
            {
                log.LogError(ex);
                throw ex;
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
        }

        private object Post()
        {
            JObject response = new JObject();
            LogEvent log = LogEvent.Start();
            var errors = new List<ApiError>();
            try
            {
                var selected = _requestHandler.GetCurrentSchemaEndpoint(ControllerContext);
                var inputs = _requestHandler.getInputs(selected, ControllerContext);
                

                //validate inputs
                errors = _requestHandler.validateInputs(inputs);
                errors.RemoveAll(e => string.IsNullOrEmpty(e.errorMessage));
                if (errors.Count == 0)
                {
                    _requestHandler.ExecuteOperations(selected, inputs);
                    _requestHandler.ManageResponseHeader(selected, ControllerContext);
                    _requestHandler.CleanStatsData();
                    _requestHandler.ValidateResponse();
                    // Handle DB Errors
                    if (_requestHandler.ResponseHttpStatusCode == 500)
                    {
                        throw new AppException(_requestHandler.ResponseData);
                    }
                }
                else
                {
                    // send 400 with respective errors
                    ErrorResponse res = new ErrorResponse { errors = new List<ApiError>() };
                    
                    if (errors.Count > 0)
                    {
                        errors.ForEach(e => res.errors.Add(new ApiError { errorMessage = e?.errorMessage, errorCode = e?.errorCode,actionToBeTaken = e?.actionToBeTaken }));
                        throw new BadRequestException(res);
                    }
                    _requestHandler.ValidateResponse();
                }
                return _requestHandler.ResponseData;
            }
            catch (AppException ex)
            {
                log.LogError(ex);
                throw ex;
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
        }
    }
}