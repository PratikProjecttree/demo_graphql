using Microsoft.AspNetCore.Mvc;
using demo_graphql.Models;
using demo_graphql.BAL.IServices;

namespace demo_graphql.Controllers
{
    [ApiController]
    [Route("api/graphql")]
    public class GraphQLController : CommonController
    {
        private readonly IGLService _graphQLService;
        public GraphQLController(IGLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLRequestModel requestModel)
        {
            var response = await _graphQLService.Post(requestModel, GetPositionId(), GetUserId());
            return Ok(response);
        }
    }
}