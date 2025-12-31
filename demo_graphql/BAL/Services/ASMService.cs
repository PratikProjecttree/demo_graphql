using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;
using demo_graphql.Core.Models;
using Microsoft.Extensions.Options;

namespace demo_graphql.BAL.Services
{
    public class ASMService : IASMService
    {
        private readonly ILogger<ASMService> _logger;
        private readonly ASMModel _asmModel;
        private readonly HttpClient _client;
        public ASMService(ILogger<ASMService> logger, HttpClient client, IOptions<ASMModel> asmModel)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client;
            _asmModel = asmModel.Value;
        }
        public async Task<List<RoleAccessViewModel>> GetAllAccessByRolePositionId(List<RolePositionModel> model)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("calling GetAllAccessByRolePositionId");
            AccessRolePositionModel requestModel = new()
            {
                ApplicationId = _asmModel.ApplicationId,
                Positions = model
            };

            HttpContent requestData = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");

            var httpResponse = await _client.PostAsync(_asmModel.Url + "application-security", requestData);
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("HTTP ASM call failed with status {StatusCode} in {Duration} ms - URL: {Url}", httpResponse.StatusCode, stopwatch.ElapsedMilliseconds, httpResponse?.RequestMessage?.RequestUri
            );
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogError(httpResponse?.RequestMessage?.RequestUri + ", 'Error' : " + httpResponse?.ReasonPhrase, null);
                    return new List<RoleAccessViewModel>();
                }

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError(httpResponse?.RequestMessage?.RequestUri + ", 'Error' : " + httpResponse?.ReasonPhrase, null);
                    throw new Exception("Cannot retrieve access");
                }
            }

            var responseString = await httpResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseString) || !responseString.TrimStart().StartsWith("{"))
            {
                _logger.LogError("Unexpected response from ASM service: {Response}", responseString);
            }

            var result = JsonConvert.DeserializeObject<AsmResponce>(responseString);
            // var result = Newtonsoft.Json.JsonConvert.DeserializeObject<AsmResponce>(await httpResponse.Content.ReadAsStringAsync());
            if (!result.Succeeded || result.Data.Count > 0)
            {

                result.Data[0].ApplicationAccess.ForEach(x =>
                {
                    x.PositionId = result.Data[0].PositionId;
                    x.RoleId = result.Data[0].RoleId;
                });
                _logger.LogInformation("HTTP ASM call completed in {Duration} ms but no data returned - URL: {Url}", stopwatch.ElapsedMilliseconds, httpResponse?.RequestMessage?.RequestUri);
                return result?.Data[0]?.ApplicationAccess;

            }
            else
            {
                return new List<RoleAccessViewModel>();
            }
        }
    }
}