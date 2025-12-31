using System.Net;
using System.Diagnostics;
using demo_graphql.Models;
using demo_graphql.BAL.IServices;
using RestSharp;
using Newtonsoft.Json;


namespace demo_graphql.BAL.Services
{
    public class MISService : IMISService
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        private readonly ILogger<MISService> _logger;
        public MISService(IRestClient client, IRestRequest request, ILogger<MISService> logger)
        {
            _request = request;
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<PositionViewModel>> GetPersonPosition(int[] personId = null)
        {
            _logger.LogInformation("Get Person Position");
            if (personId?.Length > 0)
            {
                var queryParams = "?";
                queryParams += SetQueryParams(personId, nameof(personId));
                var requestUrl = "https://api.uat.bapsapps.org/myseva/api/v1/" + "Person/Position" + queryParams;
                var response = await Execute<ListResponse<PositionViewModel>>(requestUrl);
                return response.Data;
            }
            return null;
        }
        private static string SetQueryParams(int[] param, string paramsName)
        {
            string paramString = string.Empty;
            foreach (var item in param)
            {
                paramString += paramsName + "=" + item + "&";
            }
            return paramString;
        }
        private async Task<T> Execute<T>(string url, object model = null, Method method = Method.GET)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _request.Parameters.Clear();
                _request.Resource = url;
                _request.Method = method;
                _request.AddHeader("Content-type", "application/json");
                _request.AddHeader("x-baps-auth-app-id", "F211DAC5-0DC0-4C05-B467-A407015A2BDC");
                _request.AddHeader("x-baps-auth-app-secret", "5A7BB379-B566-400F-BE77-021576B06D2F");

                if (Method.POST == method)
                {
                    _request.AddJsonBody(model);
                }
                var response = await _client.ExecuteAsync(_request);
                stopwatch.Stop();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("HTTP MIS call to {Url} succeeded in {Duration} ms", url, stopwatch.ElapsedMilliseconds);
                    // return JsonConvert.DeserializeObject<T>(response.Content);
                    return JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });

                }
                else
                {
                    _logger.LogWarning("HTTP MIS call to {Url} failed in {Duration} ms with status {StatusCode}: {Content}", url, stopwatch.ElapsedMilliseconds, response.StatusCode, response.Content);
                    throw new Exception($"MIS is unreachable or down\n{response.StatusCode}-{response.StatusDescription}\n{response.Content}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}