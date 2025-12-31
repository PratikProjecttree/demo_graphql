
using System.Text;
using System.Text.Json;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;

namespace demo_graphql.Services
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly HttpClient _httpClient;

        public WorkFlowService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> Request(WorkflowModel requestModel, Dictionary<string, object> payload)
        {
            Response _response = new();
            requestModel.payload = payload;

            // Request hasura
            var request = new HttpRequestMessage(HttpMethod.Post, requestModel.uri);

            foreach (var header in requestModel.header)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var json = JsonSerializer.Serialize(requestModel.payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = content;
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var gLResponse = await response.Content.ReadAsStringAsync();

            //Response convert
            var wfResponseModel = JsonSerializer.Deserialize<WFResponse>(gLResponse);

            // _response.data = gLResponseModel.data;
            if (!wfResponseModel.succeeded)
                _response.responseMessages = new List<ResponseMessage> { new ResponseMessage() { type = "E", message = wfResponseModel.message ?? "Workflow request failed" } };
            else
                _response.responseMessages.Add(new ResponseMessage() { type = "S", message = "Success" });

            return _response;
        }
    }
}