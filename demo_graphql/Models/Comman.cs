
namespace demo_graphql.Controllers
{
    public static class Category
    {
        public const string Default = "default";
        public const string Workflow = "workflow";
        public const string Custom = "custom";
    }
    public static class QueryType
    {
        public const string Query = "Query";
        public const string Mutation = "Mutation";
    }

    public static class AllowedUpdateOperators
    {
        public static readonly List<string> AllowedOperators = new List<string> { "_eq", "_in", "_and" };
    }
    public static class SystemModuleCode
    {
        public const string MasterEvent = "MasterEvent";
    }
    public static class ModuleAction
    {
        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
    }
    public static class OperationType
    {
        public const string Insert = "insert_";
        public const string Update = "update_";
        public const string Delete = "delete_";
        public const string Query = "Query";
    }
    public static class CommonMessage
    {
        public const string SUCCESS = "Success";
        public const string INTERNAL_SERVER_ERROR = "Internal server error";
        public const string MEMBER_NOT_EXIST = "Member data not found.";
    }
    public static class CommanDynamicParaNameForEmail
    {
        public const string eventName = "eventName";
        public const string ParticipantName = "ParticipantName";
    }
    public enum ArgRole { Unknown, Payload, Parameter }

}