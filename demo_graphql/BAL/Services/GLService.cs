
using System.Text;
using System.Text.Json;
using demo_graphql.Models;
using Microsoft.Extensions.Options;

namespace demo_graphql.Controllers
{
    public class GLService : IGLService
    {
        private readonly GraphQLConfigurationModel _config;
        private readonly HttpClient _httpClient;
        private readonly IDapperService _dapperService;

        public GLService(IOptions<GraphQLConfigurationModel> config, HttpClient httpClient, IDapperService dapperService)
        {
            _config = config.Value;
            _httpClient = httpClient;
            _dapperService = dapperService;
        }

        public async Task<Response> Post(GraphQLRequestModel requestModel, IHeaderDictionary additionalHeaders, int LoginPersonId)
        {
            Response _response = new();

            //Query validation: if not valid
            if (!GLInspector.IsValidGraphQLSyntax(requestModel.query))
            {
                _response.data = null;
                _response.responseMessage.Add(new ResponseMessage() { message = "Invalid syntax", type = "E" });
                return _response;
            }

            var queryList = GLInspector.GetTopLevelFieldNames(requestModel.query).ToArray();
            // Routing to Hasura
            var _getProcessRequest = await _dapperService.QueryFirstOrDefaultAsync<string>("select * from mds.fn_process_request(@LoginPersonId, @queryList);", new { LoginPersonId, queryList });
            var getProcessRequest = JsonSerializer.Deserialize<GLRoutingModel>(_getProcessRequest ?? "");

            // validations
            if ("workflow,custom".ToLower().Contains(getProcessRequest?.category?.ToLower()))
            {

            }

            // Request hasura
            var request = new HttpRequestMessage(HttpMethod.Post, _config.url);

            foreach (var header in _config.headers)
            {
                request.Headers.Add(header.key, header.value);
            }
            foreach (var header in additionalHeaders.Where(x => x.Key.StartsWith("hasura-", StringComparison.CurrentCultureIgnoreCase)))
            {
                request.Headers.Add(header.Key, header.Value.ToString());
            }

            var json = JsonSerializer.Serialize(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = content;
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var gLReponse = await response.Content.ReadAsStringAsync();

            var gLReponseModel = JsonSerializer.Deserialize<GLReponseModel>(gLReponse);

            _response.data = gLReponseModel.data;
            if (!gLReponseModel.succeeded)
                _response.responseMessage = gLReponseModel.errors?.Select(x => new ResponseMessage() { type = "E", message = x?.message }).ToList();
            else
                _response.responseMessage.Add(new ResponseMessage() { type = "S", message = "Success" });

            return _response;
        }
    }
}