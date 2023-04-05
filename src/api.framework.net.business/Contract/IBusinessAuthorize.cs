using Newtonsoft.Json.Linq;

namespace api.framework.net.business.Contract
{
    public interface IBusinessAuthorize
    {
        bool ValidateToken(string token, string scope);
    }
}
