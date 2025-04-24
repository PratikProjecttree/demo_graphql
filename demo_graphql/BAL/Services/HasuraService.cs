
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

        public async Task<Response> Post(GraphQLRequestModel requestModel, IHeaderDictionary additionalHeaders, int LoginPersonId)
        {
            Response _response = new();

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
            var gLResponse = await response.Content.ReadAsStringAsync();

            //Response convert
            var gLResponseModel = JsonSerializer.Deserialize<GLResponseModel>(gLResponse);

            _response.data = gLResponseModel.data;
            if (!gLResponseModel.succeeded)
                _response.responseMessages = gLResponseModel.errors?.Select(x => new ResponseMessage() { type = "E", message = x?.message }).ToList();
            else
                _response.responseMessages.Add(new ResponseMessage() { type = "S", message = "Success" });

            return _response;
        }
    }
}