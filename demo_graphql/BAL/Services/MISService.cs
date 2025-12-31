using System.Net;
using System.Diagnostics;
using demo_graphql.Models;
using demo_graphql.BAL.IServices;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;


namespace demo_graphql.Services
{
    public class MISService : IMISService
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        private readonly ILogger<MISService> _logger;
        private readonly IOptions<MisModel> _misModel;
        public MISService(IRestClient client, IRestRequest request, ILogger<MISService> logger, IOptions<MisModel> misModel)
        {
            _request = request;
            _client = client;
            _logger = logger;
            _misModel = misModel ?? throw new ArgumentNullException(nameof(misModel));
        }

        public async Task<IEnumerable<PositionViewModel>> GetPersonPosition(int[] personId = null)
        {
            _logger.LogInformation("Get Person Position");
            if (personId?.Length > 0)
            {
                var queryParams = "?";
                queryParams += SetQueryParams(personId, nameof(personId));
                var requestUrl = _misModel.Value.Url + "Person/Position" + queryParams;
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
                _request.AddHeader("x-baps-auth-app-id", _misModel.Value.AppId);
                _request.AddHeader("x-baps-auth-app-secret", _misModel.Value.AppSecret);

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