using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo_graphql.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        [NonAction]
        public int GetUserId()
        {
            return Convert.ToInt32(HttpContext.Request.HttpContext.Items["UserId"]?.ToString());
        }
        [NonAction]
        public int GetPositionId()
        {
            var headers = HttpContext.Request.Headers;

            if (!headers.ContainsKey("X-App-Position"))
            {
                throw new BadHttpRequestException("Missing required header", StatusCodes.Status400BadRequest);
            }

            var positionIdValue = headers["X-App-Position"].ToString();

            if (string.IsNullOrWhiteSpace(positionIdValue) || !int.TryParse(positionIdValue, out int positionId))
            {
                throw new BadHttpRequestException("Invalid header value", StatusCodes.Status400BadRequest);
            }

            return positionId;
        }
    }
}