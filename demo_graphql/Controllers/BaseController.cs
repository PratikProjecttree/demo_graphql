using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

public class BaseController : ControllerBase
{
    protected int LoginPersonId => GetLoginPersonId();

    private int GetLoginPersonId()
    {
        var headerLoginPersonId = HttpContext.Request.Headers["X-Login-Person-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(headerLoginPersonId)) return 0;
        try
        {
            if (headerLoginPersonId != null && int.TryParse(headerLoginPersonId, out int pid))
                return pid;

            return 0;
        }
        catch
        {
            return 0;
        }
    }
}
