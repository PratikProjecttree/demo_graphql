
using System.Text;
using System.Text.Json;
using demo_graphql.Models;
using Microsoft.Extensions.Options;

namespace demo_graphql.Controllers
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly HttpClient _httpClient;

        public WorkFlowService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> Request(WorkflowModel requestModel)
        {
            Response _response = new();

            // Request hasura
            var request = new HttpRequestMessage(HttpMethod.Get, requestModel.uri);

            foreach (var header in requestModel.header)
            {
                request.Headers.Add(header.Key, header.Value);
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