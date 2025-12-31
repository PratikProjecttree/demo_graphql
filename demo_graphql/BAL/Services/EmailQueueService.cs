using System.Data;
using Dapper;
using demo_graphql.Models;
using Npgsql;
using static demo_graphql.BAL.QueryInspector;
using demo_graphql.Models.EmailModels;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using demo_graphql.BAL.IServices;

namespace demo_graphql.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly string _connectionString;
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        private readonly ILogger<EmailQueueService> _logger;
        private readonly IOptions<CampaignApiModel> _campaignApiModel;

        public EmailQueueService(IConfiguration configuration, IRestClient client, ILogger<EmailQueueService> logger, IOptions<CampaignApiModel> campaignApiModel)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string missing.");
            _logger = logger;
            _request = new RestRequest();
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _campaignApiModel = campaignApiModel ?? throw new ArgumentNullException(nameof(campaignApiModel));
        }

        public async Task<IListResponse<EmailQueueResponse>> AddOrUpdateEmailQueue(EmailQueueRequest model)
        {
            ListResponse<EmailQueueResponse> response = new();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);

                if (!(model.EmailQueueId > 0))
                {
                    var param = new
                    {
                        email_template_id = model.EmailTemplateId,
                        title = model.Title,
                        email_queue_status_id = 1
                    };
                    model.EmailQueueId = await connection.ExecuteScalarAsync<int>(PostGresQuery.ManageEmailQueue, param);
                    if (model.EmailQueueId > 0)
                    {
                        await connection.ExecuteAsync(PostGresQuery.EmailQueueFrequencyUpdate, new { NoOfOccurence = 1, EmailQueueId = model.EmailQueueId });
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var campaign = await ExecuteSendEmailQueueMemberList(model.EmailQueueId, model.FunctionName, model.FunctionParams);
                                if (campaign.Succeeded == false)
                                {
                                    _logger.LogWarning("Email campaign failed. EmailQueueId: {EmailQueueId}, Message: {Message}", model.EmailQueueId, campaign.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Exception while executing email campaign. EmailQueueId: {EmailQueueId}", model.EmailQueueId);
                            }

                        });
                    }
                }

                response.Data = null;
                response.Message = CommonMessage.SUCCESS;
                response.Succeeded = true;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IListResponse<EmailQueueSchedularResponse>> ExecuteSendEmailQueueMemberList(int emailQueueId, string functionName, Dictionary<string, object>? functionParams = null)
        {
            ListResponse<EmailQueueSchedularResponse> response = new();
            List<EmailQueueSchedularResponse> successResponse = new();
            try
            {
                var queue = (await GetEmailQueueList(emailQueueId)).FirstOrDefault();
                var emailTypeResponse = await GetEmailTypeTemplate(queue.EmailTypeId);
                var dynamicVariablesString = "[" + string.Join(", ", emailTypeResponse.Data
                .SelectMany(et => et.DynamicVariables ?? Enumerable.Empty<EmailDynamic>())
                .Select(dv =>
                {
                    return string.IsNullOrEmpty(dv.Format) ? $"{{ \"variable\": \"{dv.Variable}\" }}" : $"{{ \"variable\": \"{dv.Variable}\", \"format\": \"{dv.Format}\" }}";
                })) + "]";
                queue.DynamicVariables = dynamicVariablesString;
                if (queue != null)
                {
                    using var connection = new NpgsqlConnection(_connectionString);
                    string sql;
                    Dictionary<string, object>? dbParams = null;

                    if (functionParams != null && functionParams.Any())
                    {
                        var rawParams = functionParams ?? new Dictionary<string, object>();
                        dbParams = ConvertJsonElements(rawParams);
                        var paramList = dbParams.Select(kv => $"{kv.Key} => @{kv.Key}");

                        sql = $"SELECT * FROM public.{functionName}({string.Join(", ", paramList)})";
                    }
                    else
                    {
                        sql = $"SELECT * FROM public.{functionName}()";
                    }

                    var memberList = await connection.QueryAsync<PersonDetail>(sql, dbParams);

                    if (memberList.Count() > 0)
                    {
                        const int batchSize = 500;
                        var totalMembers = memberList?.Count() ?? 0;
                        var batchCount = (int)Math.Ceiling((double)totalMembers / batchSize);
                        var campaignTemplate = await GetDynamicTemplateById(queue.Inbox_EmailTemplateId);
                        for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
                        {
                            var batchMembers = memberList?.Skip(batchIndex * batchSize).Take(batchSize).ToList() ?? new List<PersonDetail>();
                            var insertValues = new List<dynamic>();
                            var users = new List<UserCampaignsSendApiModel>();

                            foreach (var member in batchMembers)
                            {
                                insertValues.Add(new
                                {
                                    EmailQueueId = queue.EmailQueueId,
                                    MisId = member.PersonId,
                                    MemberId = member.PersonId,
                                    CampaignRunId = "",
                                    NoOfOccurence = queue.NoOfOccurence,
                                });

                                UserCampaignsSendApiModel user = new()
                                {
                                    misId = member.PersonId.ToString(),
                                    email = (member.Email != null) ? (member.Email?.ToString().ToLower() ?? "") : "",
                                    emailType = "PRIMARY",
                                    phoneType = "PRIMARY",
                                    bodyVars = DynamicEmailContent.SetTemplate(queue.DynamicVariables, member)
                                };
                                if (user.misId == null && string.IsNullOrEmpty(user.email))
                                {
                                    _logger.LogWarning("Skipping user with MemberId: {MemberId} as both misId and email are null or empty. While send '{Title}'.", member.PersonId, queue.Title);
                                }
                                else
                                {
                                    users.Add(user);
                                }
                            }

                            if (insertValues.Any())
                            {
                                await connection.ExecuteAsync(PostGresQuery.EmailQueueInsertWithCampaign, insertValues);
                            }

                            if (campaignTemplate != null && users.Any())
                            {
                                CampaignsSendApiModel campaignModel = new()
                                {
                                    targetApplication = null,
                                    templateId = campaignTemplate?.templateId,
                                    templateVersion = campaignTemplate?.templateVersion ?? 0,
                                    channel = campaignTemplate?.channel,
                                    users = users,
                                    expireAt = queue.EventEndDate
                                };

                                var campaignResponse = await CreateAndSendCampaign(campaignModel, queue.EmailQueueId);
                                var campaignRunId = campaignResponse?.Data.campaignRunId;
                                string errorMessage = null;
                                if (!string.IsNullOrEmpty(campaignRunId))
                                {
                                    int[] memberIds = insertValues.Select(m => (int)m.MemberId).ToArray();
                                    await connection.ExecuteAsync(PostGresQuery.EmailQueueMemberUpdate, new { CampaignRunId = campaignRunId, EmailQueueId = queue.EmailQueueId, MemberIds = memberIds });
                                    await connection.ExecuteAsync(PostGresQuery.EmailQueueUpdate, new { campaignResponse?.Data.campaignRunId, errorMessage, queue.EmailQueueId });
                                }
                                else
                                {
                                    errorMessage = JsonConvert.SerializeObject(campaignResponse);
                                    await connection.ExecuteAsync(PostGresQuery.EmailQueueUpdate, new { campaignResponse?.Data.campaignRunId, errorMessage, queue.EmailQueueId });
                                }
                                queue.ErrorMessage = CommonMessage.SUCCESS;
                                successResponse.Add(queue);
                            }
                            else
                            {
                                if (campaignTemplate == null)
                                {
                                    await connection.ExecuteAsync(PostGresQuery.EmailQueueUpdate, new { campaignRunId = "", errorMessage = $"EmailTemplate with {queue.Inbox_EmailTemplateId} is not found in inbox api", queue.EmailQueueId });
                                    queue.ErrorMessage = $"EmailTemplate with {queue.Inbox_EmailTemplateId} is not found in inbox api";
                                }
                                else
                                {
                                    queue.ErrorMessage = "No valid users to send email to.";
                                }

                                successResponse.Add(queue);
                            }
                        }
                    }
                    else
                    {
                        await connection.ExecuteAsync(PostGresQuery.EmailQueueUpdate, new { campaignRunId = "", errorMessage = CommonMessage.MEMBER_NOT_EXIST + " while Sent Email Campaign.", queue.EmailQueueId });
                        queue.ErrorMessage = CommonMessage.MEMBER_NOT_EXIST;
                        successResponse.Add(queue);
                    }
                }
                response.Data = successResponse;
                return response;
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = CommonMessage.INTERNAL_SERVER_ERROR;
                return response;
            }
        }

        public async Task<IListResponse<EmailTypeResponse>> GetEmailTypeTemplate(int? EmailTypeId)
        {
            try
            {
                ListResponse<EmailTypeResponse> list = new();
                using var connection = new NpgsqlConnection(_connectionString);

                var emailTypes = await connection.QueryAsync<EmailTypeResponse>(PostGresQuery.GetEmailType, new { EmailTypeId });
                list.Data = emailTypes;
                list.Succeeded = true;
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<EmailQueueSchedularResponse>> GetEmailQueueList(int emailQueueId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var emailQueueList = (await connection.QueryAsync<EmailQueueSchedularResponse>(PostGresQuery.GetEmailQueueList, new { EmailQueueId = emailQueueId })).ToList();
            return emailQueueList;
        }
        public async Task<CampaignTemplate> GetDynamicTemplateById(string? templateId, string approvalStatus = "APPROVED", string status = "ACTIVE")
        {
            try
            {
                var requestUrl = _campaignApiModel.Value.Url + CommonURLEndpoint.CampaignTemplates + "/" + templateId + "?approvalStatus=" + approvalStatus + "&status=" + status;
                var response = await Execute<List<CampaignTemplate>>(null, requestUrl, null, Method.GET);
                return response.FirstOrDefault();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<ISingleResponse<CampaignsResponseModel>> CreateAndSendCampaign(CampaignsSendApiModel model, int emailQueueId = 0)
        {
            try
            {
                SingleResponse<CampaignsResponseModel> result = new();
                var requestUrl = _campaignApiModel.Value.Url + CommonURLEndpoint.CampaignsSend;
                var response = await Execute<CampaignsResponseModel>(null, requestUrl, model, Method.POST, emailQueueId);
                result.Data = response;
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ISingleResponse<CampaignTemplateResponseModel>> UpdateAndCreateNewDynamicTemplateVersion(string templateId, string authorization, CampaignTemplateApiModel model)
        {
            try
            {
                SingleResponse<CampaignTemplateResponseModel> result = new();
                var requestUrl = _campaignApiModel.Value.Url + CommonURLEndpoint.CampaignTemplates + "/" + templateId;
                var response = await Execute<CampaignTemplateResponseModel>(null, requestUrl, model, Method.PUT);
                if (response != null)
                {
                    #region  :: APPROVE TEMPLATE ::
                    ApprovalTemplateModel updateModel = new()
                    {
                        approvalStatus = "APPROVED"
                    };
                    var approveRequestUrl = _campaignApiModel.Value.Url + CommonURLEndpoint.CampaignTemplates + response.templateId + "/versions/" + response.templateVersion + "/approval-status";
                    var approveData = await Execute<CampaignTemplateResponseModel>(null, approveRequestUrl, updateModel, Method.PUT);
                    #endregion

                    #region  :: ACTIVE TEMPLATE ::
                    ActiveTemplateModel activeModel = new()
                    {
                        status = "ACTIVE"
                    };
                    var activeRequestUrl = _campaignApiModel.Value.Url + CommonURLEndpoint.CampaignTemplates + response.templateId + "/versions/" + response.templateVersion + "/status";
                    var activeData = await Execute<CampaignTemplateResponseModel>(null, activeRequestUrl, activeModel, Method.PUT);
                    #endregion
                    result.Data = response;
                    result.Succeeded = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<T> Execute<T>(string authorization, string url, object model = null, Method method = Method.GET, int emailQueueId = 0)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = method;
            _request.AddHeader("Content-type", "application/json");
            _request.AddHeader("x-app-auth-id", _campaignApiModel.Value.AppId);
            _request.AddHeader("x-app-auth-secret", _campaignApiModel.Value.AppSecret);

            if (Method.POST == method || Method.PUT == method || Method.PATCH == method)
            {
                _request.AddJsonBody(model);
            }

            var response = await _client.ExecuteAsync(_request);

            if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted || (Method.DELETE == method && response.StatusCode == HttpStatusCode.NoContent))
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                if (emailQueueId > 0)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content.ToString());
                }
                var error = "Error from Campaign.\n\n" + response.StatusDescription;
                throw new ApplicationException(error);
            }
        }

        private static Dictionary<string, object> ConvertJsonElements(
    Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();

            foreach (var kv in input)
            {
                if (kv.Value is System.Text.Json.JsonElement je)
                {
                    object value = je.ValueKind switch
                    {
                        JsonValueKind.Number when je.TryGetInt32(out var i) => i,
                        JsonValueKind.Number when je.TryGetInt64(out var l) => l,
                        JsonValueKind.String => je.GetString()!,
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => DBNull.Value,
                        _ => je.ToString()!
                    };

                    result[kv.Key] = value;
                }
                else
                {
                    result[kv.Key] = kv.Value!;
                }
            }

            return result;
        }

    }
}
