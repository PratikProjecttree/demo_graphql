using demo_graphql.Models;
using FMS.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace demo_graphql.Filters
{
    public class CustomAuthorization : IAsyncAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorization> _logger;
        private readonly SsoApiModel _ssoApiModel;
        private readonly HttpClient _httpClient;

        public CustomAuthorization(ILogger<CustomAuthorization> logger,
            IOptions<SsoApiModel> ssoApiModel, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ssoApiModel = ssoApiModel.Value;
            _httpClient = httpClient;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null) return;
            var hasAllowAnonymous = filterContext.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));
            if (hasAllowAnonymous) return;

            filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens);
            var token = authTokens.FirstOrDefault();

            /*this flag for third party application call checking*/
            token = token?.Replace("Bearer ", "");

            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenInformation = handler.ReadToken(token) as JwtSecurityToken;
                if (await ValidateToken(token))
                {
                    var loginPersonId = tokenInformation?.Claims?.FirstOrDefault(claim => claim?.Type?.ToLower() == "pid")?.Value;
                    if (!string.IsNullOrEmpty(loginPersonId))
                    {
                        filterContext.HttpContext.Request.Headers["X-Login-Person-Id"] = loginPersonId;
                    }
                }
                else
                {
                    UnAuthorizeResponse(ref filterContext,
                     "1",
                     "Invalid token");
                    return;
                }
            }
            else
            {
                UnAuthorizeResponse(ref filterContext,
                    "1",
                    "Invalid token");
                return;
            }

        }
        public async Task<bool> ValidateToken(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_ssoApiModel.Url, _ssoApiModel.Endpoint.ValidateToken));
            request.Headers.Add("client-id", _ssoApiModel.ClientId);
            request.Headers.Add("Authorization", token);
            var response = await _httpClient.SendAsync(request);
            // response.EnsureSuccessStatusCode();
            return response.StatusCode == HttpStatusCode.OK;
        }
        private void UnAuthorizeResponse(ref AuthorizationFilterContext context, string errorId, string message)
        {
            context.HttpContext.Response.StatusCode = 401;
            var errorResponse = new Response();
            errorResponse.responseMessage.Add(new ResponseMessage
            {
                type = "E",
                message = message
            });
            context.Result = new UnauthorizedObjectResult(errorResponse);
            return;
        }
    }
}
