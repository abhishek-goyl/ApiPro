using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Constants = api.framework.net.Lib.Constants;
using api.framework.net.business.BusinessAuthorzation;
using api.framework.net.business.Contract;
using api.framework.net.Lib;
using api.logging;
using api.framework.net.Lib.Contracts;
using api.framework.net.Lib.Models;
using api.framework.net.Lib.Providers;

namespace api.framework.net.Filters
{
    public class ApiAuthorization : AuthorizationFilterAttribute
    {
        IConfiguration configProvider { get; set; }
        ICacheProvider cacheProvider { get; set; }
        IRequestProvider requestProvider { get; set; }
        IBusinessAuthorize authorizationProvider { get; set; }

        public ApiAuthorization(IConfiguration configProvider, ICacheProvider cacheProvider, IRequestProvider requestProvicer, IBusinessAuthorize authProvider)
        {
            this.configProvider = configProvider ?? new ConfigurationProvider();
            this.cacheProvider = cacheProvider ?? new MemoryCacheProvider();
            this.requestProvider = requestProvider ?? new RequestProvider(new SqlDataProvider(configProvider), configProvider);
            this.authorizationProvider = authProvider ?? new ApiBusinessAuthorization();
        }

        public ApiAuthorization()
        {
            this.configProvider = new ConfigurationProvider();
            this.cacheProvider = new MemoryCacheProvider();
            this.requestProvider = new RequestProvider(new SqlDataProvider(configProvider), configProvider);
            this.authorizationProvider = new ApiBusinessAuthorization();
        }

        /// <summary>
        /// Method to handle the request authorization synchronously
        /// </summary>
        /// <param name="actionContext">HttpActionContext - current action context</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Method to handle the Request Authorization Async
        /// </summary>
        /// <param name="actionContext">HttpActionContext - current action context</param>
        /// <param name="cancellationToken">CancellationToken - cancel token</param>
        /// <returns></returns>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var suthtoken = actionContext?.Request?.Headers?.Authorization?.Parameter;
            var endpoint = requestProvider.GetCurrentSchemaEndpoint(actionContext.ControllerContext);
            bool needAuth = endpoint.auth != null && endpoint.auth.Count > 0;
            var checkAuth = checkFlag(actionContext);
            if (needAuth && checkAuth)
            {
                if (actionContext.Request.Headers.Contains(Constants.AUTHORIZATION))
                {
                    // Logic to validate the token
                    if (!authorizationProvider.ValidateToken(suthtoken, endpoint.scope))
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(
                            HttpStatusCode.Unauthorized,
                            new ErrorResponse
                            {
                                errorCode = "InvalidScope",
                                errorMessage = "Invalid scope.Regenerate token and reauthenticate for accessing data."
                            });
                    }
                    return base.OnAuthorizationAsync(actionContext, cancellationToken);
                }
                else
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Content = new JsonContent(new ErrorResponse
                        {
                            errorCode = "MissingToken",
                            errorMessage = "Token not found. Regenerate token and reauthenticate for accessing data."
                        })
                    };
                    return Task.Run(() => new { });
                }
            }
            else
            {
                if (needAuth)
                {
                    createDummyIdentity(actionContext, endpoint);
                }
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }

        }

        private bool checkFlag(HttpActionContext actionContext)
        {
            bool checkAuth = true;
            try
            {
                var hasFlag = actionContext.Request.Headers.Contains(Constants.IGNORE_AUTHORIZATION_CHECK);
                var flagVal = hasFlag ? actionContext.Request.Headers.GetValues(Constants.IGNORE_AUTHORIZATION_CHECK).FirstOrDefault().TryParseString() : string.Empty;
                checkAuth = !flagVal.Equals(DateTime.Today.ToString("ddMMyyyy"));
            }
            catch { }
            return checkAuth;
        }

        private void createDummyIdentity(HttpActionContext actionContext, ApiEndpoint endpoint)
        {
            LogEvent log = LogEvent.Start();
            try
            {
                var token = actionContext?.Request?.Headers?.Authorization?.Parameter;
                var jwt = new JwtSecurityToken(token);
                var claims = new List<Claim>();


                foreach (var key in endpoint.auth)
                {
                    var data = jwt.Claims.First(c => c.Type == key);
                    claims.Add(new Claim(key.TryParseString(), data?.Value));
                }
                var id = new ClaimsIdentity(claims, "Web");
                HttpContext.Current.User = new ClaimsPrincipal(id);
            }
            catch(Exception ex)
            {
                log.LogError(ex);
            }
            finally
            {
                log.Exit();
            }
        }
    }
}