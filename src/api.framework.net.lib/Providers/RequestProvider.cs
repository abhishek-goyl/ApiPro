using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Models.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using api.framework.net.Business.BusinessValidations;
using System.IO;
using System.Security.Claims;
using System.Net.Http;
using System.Net;
using System.Globalization;
using Newtonsoft.Json;
using System.Xml.Linq;
using api.framework.net.ExceptionLib;

namespace api.framework.net.Lib.Providers
{
    public class RequestProvider : IRequestProvider
    {
        public object ResponseData { get; set; } = new JObject();
        public int ResponseHttpStatusCode { get; set; } = 200;

        private readonly IConfiguration _configuration;
        private readonly IMockSqlProvider _sqlMockHelper;
        private readonly ISqlDataProvider _sqlDataHelper;
        private readonly IBusinessProvider _businessHelper;
        private readonly IApiSchemaProvider _schemaprovider;

        public RequestProvider(ISqlDataProvider sqlDataHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlDataHelper = sqlDataHelper;
            _businessHelper = new BusinessProvider();
            _schemaprovider = new ApiSchemaProvider(configuration);
            _sqlMockHelper = new MockSqlProvider(configuration);
        }
        

        #region IRequestProvider Members

        public string GetCurrentController(HttpControllerContext context)
        {
            string controller = string.Empty;
            LogEvent log = LogEvent.Start();
            try
            {
                var routeData = context.RouteData.Values;
                controller = routeData["tag"].TryParseString();
            }
            finally
            {
                log.Exit();
            }
            return controller;
        }

        public string GetCurrentAction(HttpControllerContext context)
        {
            string action = string.Empty;
            LogEvent log = LogEvent.Start();
            try
            {
                var routeData = context.RouteData.Values;
                action = routeData["name"].TryParseString();
            }
            finally
            {
                log.Exit();
            }
            return action;
        }

        public string GetCurrentVersion(HttpControllerContext context)
        {
            string version = string.Empty;
            LogEvent log = LogEvent.Start();
            try
            {
                var routeData = context.RouteData.Values;
                version = routeData["version"].TryParseString();
            }
            finally
            {
                log.Exit();
            }
            return version;
        }

        public ApiEndpoint GetCurrentSchemaEndpoint(HttpControllerContext context, string httpMethod = null)
        {
            ApiEndpoint endpoint = new ApiEndpoint();
            LogEvent log = LogEvent.Start();
            try
            {
                httpMethod = httpMethod ?? context.Request.Method.Method;
                var controller = GetCurrentController(context);
                var action = GetCurrentAction(context);
                action = action != Constants.MOCK_ACTION ? action : string.Empty;
                var version = GetCurrentVersion(context);
                
                var lstEndpoints = _schemaprovider.GetApiEndpointsByName(controller, action, version, context);
                if (lstEndpoints.Count > 0)
                {
                    // check if HTTPVerb is supported one
                    endpoint = _schemaprovider.CheckHttpVerb(httpMethod, lstEndpoints);
                    if (endpoint == null)
                    {
                        // send 405 with proper error
                        throw new MethodNotSupportedException(string.Format("{0} method not allowed.", context.Request.Method.Method));
                    }
                    endpoint = new ApiEndpoint(endpoint);
                }
                else
                {
                    throw new NoRouteFoundException(string.Format("No implementation found for route '{0}'", context.Request.RequestUri.LocalPath));
                }
            }
            catch (AppException)
            {
                throw;
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
            return endpoint;
        }

        public  List<ApiInput> getInputs(ApiEndpoint endpoint, HttpControllerContext context)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var routeData = context.RouteData.Values;
                var queryData = HttpUtility.ParseQueryString(context.Request.RequestUri.Query);
                var headerData = context.Request.Headers;
                JObject bodyData = getBodyData(context);
                List<ApiInput> inputs = endpoint.inputs;
                if (endpoint.auth != null)
                {
                    int index = 1;
                    foreach (var claim in endpoint.auth)
                    {
                        inputs.Add(new ApiInput { name = claim, source = ApiInputSources.authClaim, value = GetClaim(context, claim), authId = index++ });
                    }
                }
                for (int i = 0; i < inputs.Count; i++)
                {
                    var input = inputs[i];
                    switch (input.source)
                    {
                        case ApiInputSources.query:
                            input.value = queryData[input.name] != null ? queryData[input.name] : getDefaultValue(input, inputs);
                            break;
                        case ApiInputSources.path:
                            input.value = routeData.ContainsKey(input.name) ? routeData[input.name] : getDefaultValue(input, inputs);
                            break;
                        case ApiInputSources.header:
                            input.value = headerData.Contains(input.name) ? headerData.GetValues(input.name).FirstOrDefault() : getDefaultValue(input, inputs);
                            break;
                        case ApiInputSources.body:
                            input.value = bodyData.ContainsKey(input.name) ? bodyData.GetValue(input.name) : getDefaultValue(input, inputs);
                        break;
                    }
                    if (input.@enum != null)
                    {
                        setEnumValue(ref input, input.value);
                    }
                }
                return inputs;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw ex;
            }
            finally
            {
                log.Exit();
            }
        }

        /// <summary>
        /// Function to read the body of the incoming request if the httpMethod is POST
        /// </summary>
        /// <param name="context">HttpControllerContext</param>
        /// <returns>JObject containg all body data in key value pair</returns>
        private JObject getBodyData(HttpControllerContext context)
        {
            JObject res = new JObject();
            if (context.Request.Method == HttpMethod.Post)
            {
                var httpContext = (HttpContextBase)context.Request.Properties["MS_HttpContext"];
                if (httpContext != null)
                {
                    var contentType = context.Request.Content.Headers.ContentType?.MediaType;
                    switch (contentType)
                    {
                        case Constants.FORM_DATA:
                            foreach (var key in httpContext.Request.Form.AllKeys)
                            {
                                res.Add(key, httpContext.Request.Form.GetValues(key).FirstOrDefault());
                            }
                            break;
                        default:
                            httpContext.Request.InputStream.Position = 0;
                            
                            using (StreamReader streamReader = new StreamReader(httpContext.Request.InputStream))
                            {
                                res = streamReader.ReadToEnd().ToJObject();
                            }
                            break;
                    }
                    foreach (var file in httpContext.Request.Files)
                    {
                        if (!res.Properties().Select(p => p.Name).Contains(file.TryParseString()))
                        {
                            res.Add(file.TryParseString(), new
                            {
                                FileName = httpContext.Request.Files[file.TryParseString()].FileName,
                                Type = httpContext.Request.Files[file.TryParseString()].ContentType ?? string.Empty
                            }.ToJSONString());
                        }
                    }
                }
            }
            return res;
        }

         private string getDefaultValue(ApiInput input, List<ApiInput> allInputs)
        {
            string value = string.Empty;
            if (!string.IsNullOrEmpty(input.defaultValue.TryParseString()))
            {
                value = getFormattedDefaultValue(input);
            }
            else if(input.conditionalDefaults != null)
            {
                // condition for conditional default
                foreach (ConditionalDefault d in input.conditionalDefaults)
                {
                    if (d.expressions.Count == 1)
                    {
                        if (d.expressions[0].Evaluate(allInputs))
                        {
                            value = getConditionalDefaultValue(d, allInputs);
                            break;
                        }
                    }
                    else
                    {
                        List<bool> lstChks = new List<bool>();
                        foreach (BooleanExpression exp in d.expressions)
                        {
                            lstChks.Add(exp.Evaluate(allInputs));
                        }
                        string conditions = string.Format(d.condition, lstChks.Select(c => c.ToString()).ToArray());
                        bool chk = conditions.EvaluateBooleanExpression();
                        if (chk)
                        {
                            value = getConditionalDefaultValue(d, allInputs);
                            break;
                        }
                    }
                }
            }
            return value;
        }

        private string getConditionalDefaultValue(ConditionalDefault def, List<ApiInput> inputs)
        {
            string value = string.Empty;
            if (!string.IsNullOrEmpty(def.fieldValue))
            {
                var field = inputs.Find(i => i.name.Equals(def.fieldValue, StringComparison.InvariantCultureIgnoreCase));
                if (field == null)
                {
                    throw new AppException(string.Format("There is no input with name {0}", def.fieldValue), "ConfigurationError");
                }
                value = field.value.TryParseString();
            }
            else if (!string.IsNullOrEmpty(def.claimValue))
            {
                var field = inputs.Find(i => i.name.Equals(def.claimValue, StringComparison.InvariantCultureIgnoreCase));
                if (field == null)
                {
                    throw new AppException(string.Format("There is no claim in token with name {0}", def.fieldValue), "ConfigurationError");
                }
                value = field.value.TryParseString();
            }
            else
            {
                value = def.value;
            }
            return value;

        }

        public List<ApiError> validateInputs(List<ApiInput> inputs)
        {
            LogEvent log = LogEvent.Start();
            List<ApiError> errors = new List<ApiError>();
            try
            {
                ExecuteValidations(inputs);

                //selecting error for inputs where ignoreValidation property is "false" and validations are not from file 
                errors = inputs
                        .Where(x => !x.ignoreValidation || x.ValidationErrors.FindAll(i => i.ExtendedProperties != null).Count == 0)
                        .SelectMany(x => x.ValidationErrors).ToList();
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw ex;
            }
            finally
            {
                log.Exit();
            }
            return errors;
        }      

        public void ExecuteOperations(ApiEndpoint endpoint, List<ApiInput> inputs)
        {
            Dictionary<string, JObject> data = new Dictionary<string, JObject>();
            foreach (var o in endpoint.operations)
            {
                switch (o.type)
                {
                    case ApiOperationTypes.query:
                    case ApiOperationTypes.sp:
                        var dRes = ResponseData = getSqlData(o, inputs, data);
                        data.Add(string.Format("operation{0}", o.order), dRes.ToJObject());
                        break;
                    case ApiOperationTypes.business:
                        var jInputs = inputs.ToJObject();
                        ResponseData = _businessHelper.ExecuteBusinessLogic(o, ResponseData.ToJObject(), ref jInputs).ToJObject();
                        inputs = JsonConvert.DeserializeObject<List<ApiInput>>(jInputs.SelectToken("data").ToJSONString());
                        break;
                }
            }
        }

        /// <summary>
        /// Method to get the response headers and add them to request context.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="context"></param>
        public void ManageResponseHeader(ApiEndpoint endpoint, HttpControllerContext context)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var headers = _schemaprovider.GetResponseHeaders(endpoint);
                foreach (var header in headers)
                {
                    context.Request.Headers.Add(header.Item1, GetStatsData(header.Item2));
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw ex;
            }
            finally
            {
                log.Exit();
            }
        }

        public string GetStatsData(string path)
        {
            LogEvent log = LogEvent.Start();
            string data = string.Empty;
            try
            {
                data = this.ResponseData.ToJObject().SelectToken(path).ToString();
            }
            catch (Exception ex)
            {
                AppException aEx = new AppException(string.Format("response does not have data at path {0}", path), "E_500", ex);
                log.LogError(aEx);
            }
            finally
            {
                log.Exit();
            }
            return data;
        }

        public void CleanStatsData()
        {
            LogEvent log = LogEvent.Start();
            try
            {
                if (this.ResponseData is JObject)
                {
                    this.ResponseData = this.ResponseData.RemoveProperty("stats");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
        }

        public void ValidateResponse()
        {
            LogEvent log = LogEvent.Start();
            try
            {
                GlobalApplicationVariables gv = new GlobalApplicationVariables(HttpContext.Current);
                if (gv.PartialRequestError?.Count > 0)
                {
                    JObject valErrors = new JObject();
                    valErrors.Add("errors", gv.PartialRequestError.ToJSONString().ToJArray());
                    if (this.ResponseData is JObject)
                    {
                        JObject temp = this.ResponseData.ToJObject();
                        temp.Merge(valErrors);
                        this.ResponseData = temp;
                    }
                }
                if (this.ResponseData is JObject)
                {
                    var props = this.ResponseData.ToJObject().Properties();
                    var errors = props.Where(p => p.Name.ToLower().Contains("error"));

                    if (errors.Count() > 0)
                    {
                        ResponseHttpStatusCode = props.Count() > 1 ? 206 : 400;
                    }
                }
                if (ResponseHttpStatusCode != 500)
                {
                    HttpContext.Current?.Items.Add(Constants.HTTP_RESPONSE_STATUS_CODE, ResponseHttpStatusCode);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
        }

        public bool IsMockRequest(out string scenario)
        {
            bool flag = false;
            LogEvent log = LogEvent.Start();
            scenario = string.Empty;
            try
            {
                flag = HttpContext.Current.Request.Headers.AllKeys.Contains(Constants.MOCK);
                bool hasScenario = HttpContext.Current.Request.Headers.AllKeys.Contains(Constants.MOCK_SCENARIO);
                if (flag && hasScenario)
                {
                    scenario = HttpContext.Current.Request.Headers[Constants.MOCK_SCENARIO].ToString();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                flag = false;
            }
            finally
            {
                log.Exit();
            }
            return flag;
        }

        #endregion

        #region Private Methods


        private void ExecuteValidations(List<ApiInput> inputs)
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                ApiInputValidation typeVal = input.validations?.Find(v => v.type.Equals(ApiInputValidationType.type));
                //this.typeCheck(ref input, typeVal);
                //    continue;
                if (input.validations != null)
                {
                    foreach (var apiVal in input.validations)
                    {
                        switch (apiVal.type)
                        {
                            case ApiInputValidationType.mandatory:
                                this.mandatoryCheck(ref input);
                                break;
                            case ApiInputValidationType.type:
                                this.typeCheck(ref input, apiVal);
                                break;
                            case ApiInputValidationType.multiExpression:
                                this.multiExpressionCheck(ref input, inputs, apiVal);
                                break;
                            case ApiInputValidationType.regex:
                                this.regexCheck(ref input, apiVal);
                                break;
                            case ApiInputValidationType.length:
                                this.lengthCheck(ref input, apiVal);
                                break;
                            case ApiInputValidationType.compare:
                                if (!string.IsNullOrEmpty(input.value.TryParseString()))
                                {
                                    this.compareCheck(ref input, apiVal, inputs);
                                }
                                break;
                            case ApiInputValidationType.range:
                                if (!string.IsNullOrEmpty(input.value.TryParseString()))
                                {
                                    this.rangeCheck(ref input, apiVal);
                                }
                                break;
                            case ApiInputValidationType.custom:
                                this.customCheck(ref input, inputs, apiVal);
                                break;
                        }
                        if (input.ValidationErrors.Count > 0)
                            break;
                    }
                }
            }
        }

        private string GetClaim(HttpControllerContext context, string key)
        {
            LogEvent log = LogEvent.Start();
            string claim = string.Empty;
            try
            {
                var principal = context?.RequestContext?.Principal as ClaimsPrincipal;
                claim = principal?.Claims?.FirstOrDefault(c => c.Type == key)?.Value;
            }
            catch (Exception ex)
            {
                UnAuthorizedException aEx = new UnAuthorizedException("Token expired or invalid.Regenerate token and reauthenticate for accessing data", "InvalidToken", ex);
                log.LogError(aEx);
                throw aEx;
            }
            finally
            {
                log.Exit();
            }
            return claim;
        }

        /// <summary>
        /// Method to get the request path path segment for the given index
        /// </summary>
        /// <param name="context">HttpControllerContext- current controler context</param>
        /// <param name="index">int- index for the required segment</param>
        /// <returns>string - request path segment</returns>
        private string GetRequestPathSegment(HttpControllerContext context, int index)
        {
            string segment = string.Empty;
            try
            {
                if (context.Request != null)
                {
                    var path = context.Request.RequestUri.LocalPath;
                    path = path.ToLower().Substring(path.IndexOf("/api/"));
                    var segments = path.Split('/');
                    if (segments.Length > index)
                    {
                        segment = segments[index];
                    }
                }
            }
            catch { }
            return segment;
        }

        /// <summary>
        /// Method to execute mandatory check
        /// </summary>
        /// <param name="input"></param>
        private void mandatoryCheck(ref ApiInput input)
        {
            try
            {
                string value = input.value.TryParseString();                
                if (string.IsNullOrEmpty(value))
                {
                    ApiInputValidation val = input.validations.Find(v => v.type.Equals(ApiInputValidationType.mandatory));
                    var errorMessage = !string.IsNullOrEmpty(val.error) ? val.error : string.Format("{0} is mandatory input.", input.name);
                    ApiError valError = new ApiError
                    {
                        errorCode = val.errorCode,
                        errorMessage = errorMessage,
                        actionToBeTaken = val.actiontobetaken
                    };
                    if (input.DataProperties != null)
                    {
                        valError.ExtendedProperties = new PostedDataProperties
                        {
                            index = input.DataProperties.index,
                            TableName = input.DataProperties.TableName
                        };
                    }
                    input.ValidationErrors.Add(valError);
                }
            }
            catch
            {
                input.ValidationErrors.Add(new ApiError
                {
                    errorCode = string.Format("Missing{0}", input.name),
                    errorMessage = string.Format("{0} is mandatory for processing this request.", input.name)
                });
            }
        }

        /// <summary>
        /// Method for type check
        /// </summary>
        /// <param name="input"></param>
        private void typeCheck(ref ApiInput input, ApiInputValidation val)
        {
            try
            {
                var strVal = input.value.TryParseString();
                string error = val != null ? val.error: "Please provide a valid {0} value for {1}";
                bool check = true;
                switch (input.type)
                {
                    case ApiInputType.@bool:
                        bool tmpBool = false;
                        check = bool.TryParse(strVal, out tmpBool);
                        break;
                    case ApiInputType.date:
                        DateTime tmpDate = DateTime.MinValue;
                        check = DateTime.TryParseExact(strVal, Constants.SUPPORTED_DATE_PATTERNS, CultureInfo.InvariantCulture,DateTimeStyles.None, out tmpDate);
                        error = string.Format(error, "date", input.name);
                        break;
                    case ApiInputType.datetime:
                        DateTime tmpDateTime = DateTime.MinValue;
                        check = DateTime.TryParseExact(strVal, Constants.SUPPORTED_DATE_PATTERNS, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDateTime);
                        error = string.Format(error, "date", input.name);
                        break;
                    case ApiInputType.@double:
                        double tmpDouble = 0;
                        check = double.TryParse(strVal, out tmpDouble);
                        break;
                    case ApiInputType.@int:
                        int tmpInt = 0;
                        check = int.TryParse(strVal, out tmpInt);
                        // check for enum
                        if (check && input.@enum != null)
                        {
                            check = input.@enum.Values.Contains(tmpInt);
                        }
                        break;
                    case ApiInputType.@long:
                        long tmpLong = 0;
                        check = long.TryParse(strVal, out tmpLong);
                        break;
                    case ApiInputType.@string:
                        if (check && input.@enum != null)
                        {
                            check = input.@enum.Keys.Where(k => k.Equals(strVal, StringComparison.InvariantCultureIgnoreCase)).Count() > 0 ;
                        }
                        break;
                }
                if (!check)
                {
                    error = string.Format(error, input.type, input.name);
                    input.ValidationErrors.Add(ApiError.GetValidationError(val, error, input.DataProperties));
                }

            }
            catch (Exception ex)
            {
                input.ValidationErrors.Add(ApiError.GetValidationError(val, string.Format("Invalid {0}, Please  provide a valid value. {1}", input.name, ex.Message), input.DataProperties));
            }
        }

        /// <summary>
        /// Method to execute the length check
        /// </summary>
        /// <param name="input">ApiInput: input object reference</param>
        /// /// <param name="val">ApiInputValidation: validation object</param>
        private void lengthCheck(ref ApiInput input, ApiInputValidation val)
        {
            try
            {
                string strValue = input.value.TryParseString();
                int length = 0;
                if (input.type == ApiInputType.@string)
                {
                    List<char> tc = strValue.ToCharArray().ToList();
                    length = tc.Count;
                }
                else if (input.type == ApiInputType.delimited)
                {
                    List<string> sc = strValue.Split(new string[] { input.separator }, StringSplitOptions.None).ToList();
                    input.value = string.Join(input.separator, sc.Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList());
                    length = sc.Count;
                }
                if ((val.minLength > 0 && val.minLength > length) || (val.maxLength > 0 && length > val.maxLength))
                {
                    var errorMessage = !string.IsNullOrEmpty(val.error) ? val.error : string.Format("Please provide a valid value for {0}.", input.name);
                    input.ValidationErrors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                }
            }
            catch (Exception ex)
            {
                input.ValidationErrors.Add(ApiError.GetValidationError(val, string.Format("length check for {0}, is failed.. {1}", input.name, ex.Message), input.DataProperties));
            }
        }

        /// <summary>
        /// Method to execute regex check
        /// </summary>
        /// <param name="input">ApiInput: input object reference</param>
        /// /// <param name="val">ApiInputValidation: validation object</param>
        private void regexCheck(ref ApiInput input, ApiInputValidation val)
        {
            try
            {
                List<string> values = new List<string>();
                List<string> invalidValues = new List<string>();
                List<ApiError> errors = new List<ApiError>();
                if (input.type == ApiInputType.delimited)
                {
                    values = input.value.TryParseString().Trim().Split(new string[] { input.separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    values.Add(input.value.TryParseString());
                }
                foreach(string s in values)
                { 
                    Match match = Regex.Match(s, val.regex, RegexOptions.IgnoreCase);
                    if (!match.Success && (!val.allowEmpty || !string.IsNullOrEmpty(s)))
                    {
                        var errorMessage = !string.IsNullOrEmpty(val.error)
                            ? string.Format(val.error, s)
                            : string.Format("{0} is an invalid {1}. Please provide a valid value for {2}.", input.value, input.name, input.name);

                        if (val.removeInvalid)
                        {
                            if (!invalidValues.Contains(s))
                            {
                                invalidValues.Add(s);
                                errors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                            }
                        }
                        else
                        {
                            // check if input already has validation error then skip
                            var chk = input.ValidationErrors.Count > 0;
                            if (!chk)
                            {
                                input.ValidationErrors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                            }
                        }
                    }
                }
                if (val.removeInvalid)
                {
                    values.RemoveAll(s => invalidValues.Contains(s));
                    input.value = string.Join(input.separator, values);
                    GlobalApplicationVariables gv = new GlobalApplicationVariables(HttpContext.Current);
                    gv.PartialRequestError = errors;

                    // check for mandatory 
                    if (input.validations.FindAll(v => v.type == ApiInputValidationType.mandatory).Count > 0
                        && values.Count == 0 && input.ValidationErrors.Count == 0)
                    {
                        input.ValidationErrors.Add(new ApiError());
                    }
                }
            }
            catch (Exception ex)
            {
                input.ValidationErrors.Add(ApiError.GetValidationError(val, string.Format("Regex check for {0} is failed. {1}", input.name, ex.Message), input.DataProperties));
            }
        }

        /// <summary>
        /// Method to perform compare check
        /// </summary>
        /// <param name="input"></param>
        /// <param name="src"></param>
        private void compareCheck(ref ApiInput input, ApiInputValidation val, List<ApiInput> inputs)
        {
            try
            {
                bool isValid = true;
                BooleanExpression exp = new BooleanExpression { inputName = input.name, value = val.value, fieldName = val.fieldName, condition = val.condition };
                isValid = exp.Evaluate(inputs);
                if (!isValid)
                {
                    var errorMessage = !string.IsNullOrEmpty(val.error) ? val.error : string.Format("Please provide a vlid value for {0}.", input.name);
                    input.ValidationErrors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                }
            }
            catch (Exception ex)
            {
                input.ValidationErrors.Add(ApiError.GetValidationError(val, string.Format("Compare check for {0} is failed. {1}", input.name, ex.Message), input.DataProperties));
            }
        }
        
        /// <summary>
        /// Method to perform range check
        /// </summary>
        /// <param name="input"></param>
        private void rangeCheck(ref ApiInput input, ApiInputValidation val)
        {
            try
            {
                bool isValid = true;
                switch (input.type)
                {
                    case ApiInputType.datetime:
                        DateTime minDate = DateTime.Parse(val.rangeStart);
                        DateTime maxDate = DateTime.Parse(val.rangeEnd);
                        DateTime valDate = DateTime.Parse(input.value.TryParseString());
                        isValid = minDate <= valDate && valDate < maxDate;
                        break;
                    case ApiInputType.@int:
                    case ApiInputType.@long:
                    case ApiInputType.@double:
                        double minDouble = double.Parse(val.rangeStart);
                        double maxDouble = double.Parse(val.rangeEnd);
                        double valDouble = double.Parse(input.value.TryParseString());
                        isValid = minDouble <= valDouble && valDouble < maxDouble;
                        break;
                    default:
                        break;
                }
                if (!isValid)
                {
                    var errorMessage = !string.IsNullOrEmpty(val.error) ? val.error : string.Format("Please provide a vlid value for {0}.", input.name);
                    input.ValidationErrors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                }
            }
            catch (Exception ex)
            {
                input.ValidationErrors.Add(ApiError.GetValidationError(val, string.Format("Range check for {0} is failed. {1}", input.name, ex.Message), input.DataProperties));
            }
        }

        private void multiExpressionCheck(ref ApiInput input, List<ApiInput> inputs, ApiInputValidation val)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                bool chk = evaluateExpressionConditions(input, inputs, val);
                if (!chk)
                {
                    var errorMessage = !string.IsNullOrEmpty(val.error) ? val.error : string.Format("Please provide a vlid value for {0}.", input.name);
                    input.ValidationErrors.Add(ApiError.GetValidationError(val, errorMessage, input.DataProperties));
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                input.ValidationErrors.Add(new ApiError
                {
                    errorCode = val.errorCode,
                    errorMessage = val.error
                });
            }
            finally
            {
                log.Exit();
            }
        }

        /// <summary>
        /// Method to evaluate the Expressions
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputs"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool evaluateExpressionConditions(ApiInput input, List<ApiInput> inputs, ApiInputValidation val)
        {
            List<bool> lstChks = new List<bool>();
            var rowIndex = input.DataProperties != null ? input.DataProperties.index : -1;
            foreach (BooleanExpression cv in val.expressions)
            {
                lstChks.Add(cv.Evaluate(inputs, rowIndex));
            }

            string conditions = string.Format(val.condition, lstChks.Select(c => c.ToString()).ToArray());
            bool chk = conditions.EvaluateBooleanExpression();
            return chk;
        }

        /// <summary>
        /// Method to perform custom / business check
        /// </summary>
        /// <param name="input"></param>
        private void customCheck(ref ApiInput input, List<ApiInput> inputs, ApiInputValidation val)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                ValidationFactory factory = new ValidationFactory(val.name);
                var chk = factory._validation.Validate(new { inputs = inputs }.ToJObject());
                if (!chk)
                {
                   input.ValidationErrors.Add(ApiError.GetValidationError(val, val.error, input.DataProperties));
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                input.ValidationErrors.Add(new ApiError
                {
                     errorCode = val.errorCode,
                     errorMessage = val.error
                });
            }
            finally
            {
                log.Exit();
            }
        }

        private JObject getSqlData(ApiOperation operation, List<ApiInput> inputs, Dictionary<string, JObject> data)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var response = new JObject();
                List<List<SqlParameter>> parameters = GetSqlParameters(operation, inputs, data);
                foreach (var parameter in parameters)
                {
                    JObject res = new JObject();
                    var ds = new DataSet();
                    string mock_scenario = string.Empty;
                    bool isMock = IsMockRequest(out mock_scenario);
                    if (operation.type == ApiOperationTypes.sp)
                    {
                        ds = isMock
                            ? _sqlMockHelper.ExecuteProcedure(operation.value, mock_scenario, parameter)
                            : _sqlDataHelper.ExecuteProcedure(operation.db, operation.value, parameter.ToArray());
                        log.Debug("get data");

                    }
                    else if (operation.type == ApiOperationTypes.query)
                    {
                        ds = _sqlDataHelper.ExecuteQuery(operation.db, operation.value, parameter.ToArray());
                    }

                    // format errors
                    for(int i = 1; i < ds.Tables.Count; i ++)
                    {
                        if (ds.Tables[i].Columns.Contains("ErrorMessage"))
                        {
                            foreach (DataRow r in ds.Tables[i].Rows)
                            {
                                r["ErrorMessage"] = string.Format(r["ErrorMessage"].TryParseString(), r.ItemArray);
                            }
                        }
                    }

                    if (operation.output == null)
                    {
                        foreach (DataTable table in ds.Tables)
                        {
                            var strTable = table.ToJSONString();
                            res = strTable.ToJObject();
                        }
                    }
                    else
                    {
                        var tables = operation.output.Properties().Select(p => p.Name).ToList();
                        var tindex = 0;
                        List<JProperty> elements = new List<JProperty>();
                        foreach (var table in tables)
                        {
                            var columns = operation.output.GetValue(table).ToObject<List<string>>();
                            JArray d = new JArray();
                            foreach (DataRow r in ds.Tables[tindex].Rows)
                            {
                                List<JProperty> properties = new List<JProperty>();
                                foreach (var col in columns)
                                {
                                    properties.Add(new JProperty(col.ToCamelCase(), r.GetValue<object>(col)));
                                }
                                JObject item = new JObject(properties);
                                d.Add(item);
                            }
                            tindex++;
                            if (d.Count() > 0)
                            {
                                JProperty tProp = new JProperty(table.ToCamelCase(), d);
                                elements.Add(tProp);
                            }
                            log.Debug("serialized datatable");
                        }
                        res = new JObject(elements);
                    }
                    response.Merge(res, new JsonMergeSettings
                    {
                        // union array values together to avoid duplicates
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                }
                return response;
            }
            finally
            {
                log.Exit();
            }
        }

        private List<List<SqlParameter>> GetSqlParameters(ApiOperation operation, List<ApiInput> inputs, Dictionary<string, JObject> data)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                List<List<SqlParameter>> parameters = new List<List<SqlParameter>>();
                List<SqlParameter> list = new List<SqlParameter>();
                parameters.Add(list);
                foreach (var input in operation.inputs)
                {
                    var dnType = getSQLType(input.type.ToString());
                    object value = string.Empty;
                    switch (input.source)
                    {
                        case DbInputSources.authClaim:
                            value = getAuthInputValue(input, inputs);
                            if (!string.IsNullOrEmpty(value.TryParseString()))
                            {
                                SqlParameter clp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = value };
                                parameters.ForEach(l => l.Add(clp));
                            }
                            break;
                        case DbInputSources.none:
                            value = convertToDbType(input.defaultValue, input.type);                           
                            SqlParameter ncp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = value };
                            parameters.ForEach(l => l.Add(ncp));
                            break;
                        case DbInputSources.client:
                            value = getApiInputValue(input, inputs);
                            if (!string.IsNullOrEmpty(value.TryParseString()))
                            {
                                SqlParameter cp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = value };
                                parameters.ForEach(l => l.Add(cp));
                            }
                            break;
                        case DbInputSources.data:
                            List<object> values = getDataInputValue(input, data);
                            if (values.Count > 1)
                            {
                                for (var i = 1; i < values.Count; i++)
                                {
                                    var newList = new List<SqlParameter>();
                                    list.ForEach(c => newList.Add(c));
                                    parameters.Add(newList);
                                }
                                int index = 0;
                                foreach (var v in values)
                                {
                                    SqlParameter dp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = v };
                                    parameters[index++].Add(dp);
                                }
                            }
                            else if (values.Count == 1)
                            {
                                SqlParameter sdp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = values[0] };
                                parameters.ForEach(l => l.Add(sdp));
                            }
                            break;
                        case DbInputSources.file:
                            var dataParts = input.mapId.Split('.');
                            ApiInput iInput = inputs.Where(x => x.name.Equals(dataParts[0])).SelectMany(x => x.FileInputs).Where(x => x.name.Equals(dataParts[1])).FirstOrDefault();
                            value = iInput?.value;
                            SqlParameter fcp = new SqlParameter { ParameterName = input.name, SqlDbType = dnType, Value = value };
                            parameters.ForEach(l => l.Add(fcp));
                            break;
                    }
                }
                // remove empty parameter array 
                parameters.RemoveAll(p => p.Count == 0);
                log.Debug(string.Format("total number of executions: {0}", parameters.Count));
                return parameters;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
        }

        private SqlDbType getSQLType(string inputType)
        {
            SqlDbType t = SqlDbType.NVarChar;
            if (Enum.TryParse<SqlDbType>(inputType, out t))
            {
                return t;
            }
            switch (inputType.ToLower())
            {
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "ntext":
                    return SqlDbType.NText;
                case "bigint":
                    return SqlDbType.BigInt;
                case "bit":
                    return SqlDbType.Bit;
                case "char":
                    return SqlDbType.Char;
                case "datetime":
                    return SqlDbType.DateTime;
                case "date":
                    return SqlDbType.Date;
                case "decimal":
                    return SqlDbType.Decimal;
                case "int":
                    return SqlDbType.Int;
                default:
                    return SqlDbType.NVarChar;
            }
        }

        /// <summary>
        /// Method to get the SQL parameter value from the output/response of other SQL operation
        /// </summary>
        /// <param name="dbInput">DbInput - details for input</param>
        /// <param name="data">Dictionary<string, JObject> - responses for other Operations</param>
        /// <returns></returns>
        private List<object> getDataInputValue(DbInput dbInput, Dictionary<string, JObject> data)
        {
            LogEvent log = LogEvent.Start();
            var values = new List<object>();
            try
            {
                string[] parts = dbInput.mapId.Split('.');
                JObject res = data[parts[0]];
                var resRows = res.GetValue(parts[1]);
                if (resRows is JArray)
                {
                    foreach (JObject row in (JArray)resRows)
                    {
                        var strVal = row.GetValue(parts[2]).ToString();
                        var val = convertToDbType(strVal, dbInput.type);
                        if (!values.Contains(val))
                        {
                            values.Add(val);
                        }
                    }
                    if (dbInput.allowMultiple)
                    {
                        var strValue = string.Join(",", values);
                        values.Clear();
                        values.Add(strValue);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
            return values;
        }

        private object getAuthInputValue(DbInput dbInput, List<ApiInput> inputs)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                ApiInput apiInput = inputs.Find(i => i.authId.ToString().Equals(dbInput.mapId));
                if (apiInput != null)
                {
                    return convertToDbType(apiInput.value.TryParseString(), dbInput.type);
                }
                else
                {
                    throw new Exception("Claim Input not provided");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
        }

        private object getApiInputValue(DbInput dbInput, List<ApiInput> inputs)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                ApiInput apiInput = inputs.Find(i => i.id.ToString().Equals(dbInput.mapId));
                if (apiInput == null)
                {
                    throw new Exception(string.Format("API Input not provided for {0}", dbInput.name));
                }
                string value = apiInput.value.TryParseString();
                if (apiInput.@enum != null)
                {
                    value = (dbInput.type == DbTypes.Integer || dbInput.type == DbTypes.SmallInt) 
                        ? apiInput.enumInt.TryParseString() 
                        : apiInput.enumString;
                }
                return convertToDbType(value, dbInput.type);
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                throw;
            }
            finally
            {
                log.Exit();
            }
        }

        private object convertToDbType(string value, DbTypes type)
        {
            var res = value;
            switch (type)
            {
                case DbTypes.SmallInt:
                    break;
                case DbTypes.Nvarchar:
                    break;
            }
            return value;
        }

        private string getFormattedDefaultValue(ApiInput input)
        {
            string value = string.Empty;
            switch (input.type)
            {
                case ApiInputType.date:
                case ApiInputType.datetime:
                    value = Utility.getDateForConstants(input.defaultValue.TryParseString(), input.pattern);
                    break;
                default:
                    value = input.defaultValue.TryParseString();
                    break;
            }
            return value;
        }

        private string getFormattedValue(string inputValue, ApiInputType type, string pattern = "")
        {
            string value = string.Empty;
            DateTime tmpDate = DateTime.Now;
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            string[] formats = cultureInfo.DateTimeFormat.GetAllDateTimePatterns();
            switch (type)
            {
                case ApiInputType.datetime:
                case ApiInputType.date:
                    if (!string.IsNullOrEmpty(pattern) && DateTime.TryParseExact(inputValue.TryParseString(), Constants.SUPPORTED_DATE_PATTERNS, cultureInfo, DateTimeStyles.None, out tmpDate))
                    {
                        value = tmpDate.ToString(pattern);
                    }
                    else if (string.IsNullOrEmpty(pattern) && DateTime.TryParse(inputValue, out tmpDate))
                    {
                        value = tmpDate.ToString(pattern);
                    }
                    else
                    {
                        value = inputValue;
                    }
                    break;
                default:
                    return inputValue;
            }
            return value;
        }

        private void setEnumValue(ref ApiInput input, object value)
        {
            if (input.@enum != null)
            {
                switch (input.type)
                {
                    case ApiInputType.@int:
                        foreach (var pair in input.@enum)
                        {
                            if (pair.Value.TryParseString().Equals(value.TryParseString(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                int tmpInt = 0;
                                int.TryParse(pair.Value.TryParseString(), out tmpInt);
                                input.enumInt = tmpInt;
                                input.enumString = pair.Key;
                                input.value = pair.Value.TryParseString();
                            }
                        }
                        break;
                    case ApiInputType.@string:
                        string key = value.TryParseString();
                        input.value = input.enumString = key;
                        List<string> lowerKeys = input.@enum.Keys.Select(k => k.ToLower()).ToList();
                        if (lowerKeys.Contains(key.ToLower()))
                        {
                            var val = input.@enum.Where(k => k.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                            input.enumInt = val.Select(k => k.Value).FirstOrDefault();
                        }
                        break;
                }
            }
        }

        public object GetXMLResponse(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            string dsXml = ds.GetXml();
            dsXml = HttpUtility.HtmlDecode(dsXml);
            return dsXml;            
        }

        #endregion
    }
}
