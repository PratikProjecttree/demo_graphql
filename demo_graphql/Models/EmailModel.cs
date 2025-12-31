using System.Text.Json.Serialization;
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
    public class EmailQueueMemberResponse
    {
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string? EventLocation { get; set; }
        public string? EventAddress { get; set; }
        public DateTime? CloseParticipantRegistrationDate { get; set; }
        public DateTime? OpenParticipantRegistrationDate { get; set; }
        public DateTime? DueTransportationDate { get; set; }
        public DateTime? DueAccomodationDate { get; set; }
        public decimal? OutStandingAmount { get; set; }
        public string? CurrencyType { get; set; }
        public string? TargetApplication { get; set; }
        public int MemberId { get; set; }
        public int? MisId { get; set; }
        public string? Email { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string? MemberZone { get; set; }
        public string? MemberCenter { get; set; }
        public string? BAPSID { get; set; }
        public bool allowQrcode { get; set; }
        public string? SessionName { get; set; }
        private string? _familyDetail { get; set; }
        public DateTime? TripStartDate { get; set; }
        public DateTime? TripEndDate { get; set; }
        // public List<FamilyShortInfoModel>? FamilyReponse
        // {
        //     get => _familyDetail != null ? JsonConvert.DeserializeObject<List<FamilyShortInfoModel>>(_familyDetail) : new List<FamilyShortInfoModel>();
        //     set { _familyDetail = value != null ? JsonConvert.SerializeObject(value) : null; }
        // }
        private string? _groupPerson { get; set; }
        // public List<GroupPersonResponse>? GroupPerson
        // {
        //     get => _groupPerson != null ? JsonConvert.DeserializeObject<List<GroupPersonResponse>>(_groupPerson) : new List<GroupPersonResponse>();
        //     set { _groupPerson = value != null ? JsonConvert.SerializeObject(value) : null; }
        // }
        private string? _QuestionAnswer { get; set; }
        // public List<EmailQuestionResponse>? QuestionAnswer
        // {
        //     get => _QuestionAnswer != null ? JsonConvert.DeserializeObject<List<EmailQuestionResponse>>(_QuestionAnswer) : new List<EmailQuestionResponse>();
        //     set { _QuestionAnswer = value != null ? JsonConvert.SerializeObject(value) : null; }
        // }
        public string? DietaryRestrictions { get; set; }
        public string? OtherRestrictions { get; set; }
        public string? TransportationReqd { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public TimeSpan? ArrivalTime { get; set; }
        public string? ArrivalAirport { get; set; }
        public string? ArrivalAirline { get; set; }
        public string? ArrivalFlightNumber { get; set; }
        public DateTime? DepartureDate { get; set; }
        public TimeSpan? DepartureTime { get; set; }
        public string? DepartureAirport { get; set; }
        public string? DepartureAirline { get; set; }
        public string? DepartureFlightNumber { get; set; }
        public string? AccommodationType { get; set; }
        public bool IsRsvpEvent { get; set; }
        public int? EventTypeId { get; set; }
        public string? DriverName { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleMakeName { get; set; }
        public string? VehicleModelName { get; set; }
        public string? VehicleColor { get; set; }
        public string? RequestType { get; set; }
        public string? _TripDetails { get; set; }
        public int? TripNumber { get; set; }
        public int? TripId { get; set; }
        // public List<ParticipantFlightInfo>? TripDetails
        // {
        //     get => _TripDetails != null ? JsonConvert.DeserializeObject<List<ParticipantFlightInfo>>(_TripDetails) : new List<ParticipantFlightInfo>();
        //     set { _TripDetails = value != null ? JsonConvert.SerializeObject(value) : null; }
        // }
        public int? RsvpMemberId { get; set; }
        public string? Pin { get; set; }
        public string? PreviewUrl { get; set; }
        public string? MemberGroupName { get; set; }
    }
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
    public class DynamicVariables
    {
        public string? Variable { get; set; }
        public string? Format { get; set; }
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

}