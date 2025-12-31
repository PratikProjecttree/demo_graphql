using System.Net;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace demo_graphql.Filters
{
    public class AccessPermissionAuthorization : TypeFilterAttribute
    {
        public AccessPermissionAuthorization(string[] moduleCode, string[] moduleActions) : base(typeof(AccessAuthorization))
        {
            Arguments = new object[] { moduleCode, moduleActions };
        }
        private class AccessAuthorization : IAsyncActionFilter
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IAuthenticationService _authenticationProvider;
            public string[] ModuleCode { get; set; }
            public string[] ModuleActions { get; set; }
            public AccessAuthorization(IHttpContextAccessor httpContextAccessor, IAuthenticationService authenticationProvider, string[] moduleCode, string[] moduleActions)
            {
                _httpContextAccessor = httpContextAccessor;
                ModuleCode = moduleCode;
                ModuleActions = moduleActions;
                _authenticationProvider = authenticationProvider;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                int userId = 0;
                int positionId = 0;

                var userObj = _httpContextAccessor.HttpContext?.Items["UserId"];
                if (userObj != null && int.TryParse(userObj.ToString(), out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                // PositionId from header X-App-Position
                var posHeader = _httpContextAccessor.HttpContext?.Request?.Headers["X-App-Position"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(posHeader) && int.TryParse(posHeader, out var parsedPositionId))
                {
                    positionId = parsedPositionId;
                }

                var moduleAccessResponse = await _authenticationProvider.PermissionByPosition(positionId, userId);
                IEnumerable<UserModuleAccessResponse> moduleAccessList = moduleAccessResponse ?? new List<UserModuleAccessResponse>();

                if (!moduleAccessList.Any())
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Result = new UnauthorizedResult();
                    return;
                }

                if (moduleAccessList.Any())
                {
                    if (!moduleAccessList.Any(t => ModuleCode.Contains(t.ModuleCode) && ModuleActions.Contains(t.ActionCode)))
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                        return;
                    }
                }

                await next();
            }
        }
    }
}