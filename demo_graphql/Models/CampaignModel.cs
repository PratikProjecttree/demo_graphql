using Newtonsoft.Json;
namespace demo_graphql.Models
{

    public class CampaignTemplate
    {
        public string templateId { get; set; }
        public string templateName { get; set; }
        public int templateVersion { get; set; }
        public string title { get; set; }
        public string previewBody { get; set; }
        public string body { get; set; }
        public string bodyFormat { get; set; }
        public List<dynamic> previewBodyVars { get; set; }
        public List<dynamic> bodyVars { get; set; }
        public object metadata { get; set; }
        public string channel { get; set; }
        public string approvalStatus { get; set; }
        public string status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? lastModifiedAt { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
    }
    public class CampaignsSendApiModel
    {
        public string? targetApplication { get; set; }
        public string? templateId { get; set; }
        public int templateVersion { get; set; }
        public string? channel { get; set; }
        public DateTime? expireAt { get; set; }
        public List<UserCampaignsSendApiModel>? users { get; set; }
    }
    public class UserCampaignsSendApiModel
    {
        public string? misId { get; set; }
        public string? email { get; set; }
        public string? emailType { get; set; }
        public string? phoneType { get; set; }
        public object? bodyVars { get; set; }
    }
    public class CampaignTemplateResponseModel
    {
        public string approvalStatus { get; set; }
        public List<dynamic> bodyVars { get; set; }
        public string channel { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? lastModifiedAt { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public object metadata { get; set; }
        public string previewBody { get; set; }
        public List<dynamic> previewBodyVars { get; set; }
        public string status { get; set; }
        public string templateId { get; set; }
        public string templateName { get; set; }
        public int templateVersion { get; set; }
        public string title { get; set; }
    }
    public class CampaignsResponseModel
    {
        public string? campaignRunId { get; set; }
        public object? error { get; set; }
    }

    public class ApprovalTemplateModel
    {
        public string approvalStatus { get; set; }
    }
    public class ActiveTemplateModel
    {
        public string status { get; set; }
    }
    public class CampaignTemplateApiModel
    {
        public string? templateName { get; set; }
        public string? title { get; set; }
        public string? previewBody { get; set; }
        public string? body { get; set; }
        public string? bodyFormat { get; set; }
        public string? channel { get; set; }
    }
    public class CampaignApiModel
    {
        public string Url { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public CampaignEndpoint Endpoint { get; set; }
    }
    public class CampaignEndpoint
    {
        public string Templates { get; set; }
        public string CampaignsSend { get; set; }
    }
}