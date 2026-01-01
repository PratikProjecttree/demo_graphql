
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

            //  Normalize payload globally (Hasura → Activepieces)
            if (ContainsEqOperator(payload))
            {
                var normalizedPayload = NormalizeToActivepiecesPayload(payload);
                requestModel.payload = normalizedPayload;
            }
            else
            {
                requestModel.payload = payload;
            }

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
        private Dictionary<string, object> NormalizeToActivepiecesPayload(Dictionary<string, object> input)
        {
            var where = new Dictionary<string, object>();
            var data = new Dictionary<string, object>();

            foreach (var kv in input)
            {
                // Hasura-style: field: { _eq: value }
                if (kv.Value is Dictionary<string, object> obj &&
                    obj.TryGetValue("_eq", out var eqVal))
                {
                    where[kv.Key] = ConvertNumberIfPossible(eqVal);
                }
                else
                {
                    data[kv.Key] = ConvertNumberIfPossible(kv.Value);
                }
            }

            var result = new Dictionary<string, object>();

            if (where.Count > 0)
                result["where"] = where;

            result["data"] = data;

            return result;
        }
        private object ConvertNumberIfPossible(object value)
        {
            if (value is string s && int.TryParse(s, out var i))
                return i;

            return value;
        }
        private bool ContainsEqOperator(Dictionary<string, object> payload)
        {
            foreach (var value in payload.Values)
            {
                if (value is IDictionary<string, object> obj &&
                    obj.ContainsKey("_eq"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}