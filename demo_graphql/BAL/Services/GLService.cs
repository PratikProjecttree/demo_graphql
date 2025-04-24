using System.Text.Json;
using demo_graphql.Models;
using static demo_graphql.Controllers.QueryInspector;

namespace demo_graphql.Controllers
{
    public class GLService : IGLService
    {
        private readonly IDapperService _dapperService;
        private readonly IHasuraService _hasuraService;

        public GLService(IDapperService dapperService, IHasuraService hasuraService)
        {
            _dapperService = dapperService;
            _hasuraService = hasuraService;
        }

        public async Task<Response> Post(GraphQLRequestModel requestModel, IHeaderDictionary additionalHeaders, int LoginPersonId)
        {
            Response _response = new();

            //Query validation: if not valid
            if (!GLInspector.IsValidGraphQLSyntax(requestModel.query))
            {
                _response.data = null;
                _response.responseMessages.Add(new ResponseMessage() { message = "Invalid syntax", type = "E" });
                return _response;
            }
            // get all query list
            var queryList = GLInspector.GetTopLevelFieldNames(requestModel.query).ToArray();

            // Routing to Hasura
            var _getProcessRequest = await _dapperService.QueryFirstOrDefaultAsync<string>(PostGresQuery.fn_process_request, new { LoginPersonId, queryList });
            var getProcessRequest = JsonSerializer.Deserialize<GLRoutingModel>(_getProcessRequest ?? "");

            // validations
            var categories = new HashSet<string>
            {
                Category.Workflow,
                Category.Custom,
            };
            if (categories.Contains(getProcessRequest?.category?.ToLower()))
            {

            }

            //hasura request
            _response = await _hasuraService.Post(requestModel, additionalHeaders, LoginPersonId);

            return _response;
        }
    }
}