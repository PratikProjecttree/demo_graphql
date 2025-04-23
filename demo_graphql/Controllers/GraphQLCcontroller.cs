using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GraphQLParser;
using GraphQLParser.Exceptions;
using demo_graphql.Models;

namespace demo_graphql.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphQLController : BaseController
    {
        private readonly IGLService _graphQLService;
        public GraphQLController(IGLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(GraphQLRequestModel requestModel)
        {
            var response = await _graphQLService.Post(requestModel, Request.Headers, LoginPersonId);
            return Ok(response);
        }
    }
}