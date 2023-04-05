using api.framework.net.business.Contract;
using api.framework.net.ExceptionLib;
using api.logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;

namespace api.framework.net.business.BusinessAuthorzation
{
    public class ApiBusinessAuthorization : IBusinessAuthorize
    {
        const string PUBLIC_KEY_CACHE_EXP = "KeyCacheExp";
        const string PUBLIC_KEY = "PublicAuthKey";
        const string KEY_SIZE = "KeySize";
        const string ISSUER = "myCompany";

        public bool ValidateToken(string token, string scope)
        {
            LogEvent log = LogEvent.Start();
            try
            {          
                var cacheExp = ConfigurationManager.AppSettings[PUBLIC_KEY_CACHE_EXP];
                var publicKeyFile = ConfigurationManager.AppSettings[PUBLIC_KEY];
                var issuer = ConfigurationManager.AppSettings[ISSUER];
                var keySize = ConfigurationManager.AppSettings[KEY_SIZE];
                // get public key for the incoming key id header
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;
                if (tokenS.Header.ContainsKey("kid"))
                {
                    var jwtHeader = tokenS.Header.First(h => h.Key.Equals("kid", StringComparison.InvariantCultureIgnoreCase)).Value;
                    var kid = jwtHeader.ToString();
                    // write logic to fetch the current public key

                }
                var rsa = new RSACryptoServiceProvider(int.Parse(keySize));
                var publicKey = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, publicKeyFile));
                rsa.FromXmlString(publicKey);
                var validationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = issuer,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };

                SecurityToken validatedToken;
                HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                log.Debug("OAuthValidationAttribute:: ValidateTokenAndScope-Token Validated");
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                var result = identity.HasClaim(x => x.Type == "scopes" && x.Value.ToLower().Split(',').Contains(scope.ToLower()));
                log.Debug("OAuthValidationAttribute:: ValidateTokenAndScope-Scope Validated");
                return result;
            }
            catch (Exception ex)
            {
                throw new UnAuthorizedException("Token expired or invalid.Regenerate token and reauthenticate for accessing data.", "InvalidToken", ex);
            }
            finally
            {
                log.Exit();
            }
        }
    }
}
