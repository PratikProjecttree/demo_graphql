using Newtonsoft.Json;
namespace demo_graphql.Models.EmailModels
{
    public class EmailQueueRequest
    {
        public bool IsEnableEmail { get; set; }
        public int EmailQueueId { get; set; }
        public int EmailTemplateId { get; set; }
        public string? Title { get; set; }
        public string FunctionName { get; set; } = null!;

        // dynamic params
        public Dictionary<string, object>? FunctionParams { get; set; }
    }

    public class EmailQueueResponse
    {
        public int EmailQueueId { get; set; }
        public int EmailTemplateId { get; set; }
        public string EmailTemplate { get; set; } = null!;
        public int? MemberListTypeId { get; set; }
        public string MemberListType { get; set; } = null!;
        public string? Title { get; set; }
        public string? PersonIds { get; set; }
        public int EmailQueueStatusId { get; set; }
        public string? EmailQueStatus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }
        public int? EmailTypeId { get; set; }
        public string? FromId { get; set; }
        public string? ToId { get; set; }
        public string? CampaignId { get; set; }
        public string? Gender { get; set; }
        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public int? OccurenceType { get; set; }
        public string? OccurenceDuration { get; set; }
        public bool? IsActive { get; set; }
        public int? NoOfOccurence { get; set; }
        public bool? SystemGenerated { get; set; }
        public string? TagIds { get; set; }
    }
    public class EmailQueueSchedularResponse
    {
        public int EmailQueueId { get; set; }
        public string? EmailQueue { get; set; }
        public int EmailTemplateId { get; set; }
        public string EmailTemplate { get; set; } = null!;
        public int EventId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? BodyFormate { get; set; }
        public string? Channel { get; set; }
        public int EmailQueueStatusId { get; set; }
        public string? EmailQueStatus { get; set; }
        public int EmailTypeId { get; set; }
        public string? EmailType { get; set; }
        public int? MemberListTypeId { get; set; }
        public string MemberListType { get; set; } = null!;
        public string? PersonIds { get; set; }
        public string? EmailIds { get; set; }
        public int? MemberGroupId { get; set; }
        public string? MemberGroup { get; set; }
        public string? Title { get; set; }
        public string? DynamicVariables { get; set; }
        public string? Inbox_EmailTemplateId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? TargetApplication { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? Gender { get; set; }
        public string? TagIds { get; set; }
        public int? NoOfOccurence { get; set; }
    }
    public class EmailTypeResponse
    {
        public byte EmailTypeId { get; set; }

        public string Title { get; set; } = null!;
        private string? _dynamic { get; set; }
        public List<EmailDynamic>? DynamicVariables
        {
            get => _dynamic != null ? JsonConvert.DeserializeObject<List<EmailDynamic>>(_dynamic) : null;
            set { _dynamic = value != null ? JsonConvert.SerializeObject(value) : null; }
        }
    }
    public class EmailDynamic
    {
        public string? Variable { get; set; }
        public string? Format { get; set; }
        public bool hideForRsvp { get; set; }
    }
    public class PersonDetail
    {
        public int PersonId { get; set; }
        public string? Email { get; set; }
        public string? EventName { get; set; }
        public string? Name { get; set; }
    }
    public class DynamicVariables
    {
        public string? Variable { get; set; }
        public string? Format { get; set; }
    }

}