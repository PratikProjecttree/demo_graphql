
using System.Text;
using System.Text.Json;
using demo_graphql.Models;
using Microsoft.Extensions.Options;

namespace demo_graphql.Controllers
{
    public class HasuraService : IHasuraService
    {
        private readonly GraphQLConfigurationModel _config;
        private readonly HttpClient _httpClient;

        public HasuraService(IOptions<GraphQLConfigurationModel> config, HttpClient httpClient)
        {
            _config = config.Value;
            _httpClient = httpClient;
        }

        public async Task<Response> Post(GraphQLRequestModel requestModel)
        {
            Response _response = new();

            // Request hasura
            var request = new HttpRequestMessage(HttpMethod.Post, _config.url);

            foreach (var header in _config.headers ?? [])
            {
                request.Headers.Add(header.key, header.value);
            }

            object gqlBody;
            if (string.IsNullOrWhiteSpace(requestModel.variables))
            {
                gqlBody = new
                {
                    query = requestModel.query,
                    variables = (object?)null,
                    operationName = requestModel.operationName
                };
            }
            else
            {
                // variables string -> JsonElement so it becomes a JSON object
                var vars = JsonSerializer.Deserialize<JsonElement>(requestModel.variables);

                gqlBody = new
                {
                    query = requestModel.query,
                    variables = (object)vars,
                    operationName = requestModel.operationName
                };
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(gqlBody, options);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var gLResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _response.data = null;
                _response.responseMessages = new List<ResponseMessage>
                {
                    new ResponseMessage { type = "E", message = gLResponse }
                };
                return _response;
            }

            var gLResponseModel = JsonSerializer.Deserialize<GLResponseModel>(gLResponse);

            _response.data = gLResponseModel?.data;
            if (!gLResponseModel.succeeded)
                _response.responseMessages = gLResponseModel.errors?
                    .Select(x => new ResponseMessage { type = "E", message = x?.message }).ToList();
            else
                _response.responseMessages.Add(new ResponseMessage { type = "S", message = "Success" });

            return _response;
        }
    }
}