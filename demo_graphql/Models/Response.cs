using System.Net;
using demo_graphql.Controllers;

namespace demo_graphql.Models
{
    public class Response
    {
        public object? data { get; set; }
        public List<ResponseMessage> responseMessages { get; set; } = new List<ResponseMessage>();
    }

    public class ResponseMessage
    {
        public Guid? id { get; set; }
        public string? type { get; set; }
        public string? message { get; set; }
        public int statusCode { get; set; }
    }
    public class Errors
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorId { get; set; }
    }
    public interface IResponse
    {
        bool Succeeded { get; set; }
        List<Errors> Errors { get; set; }
        string Message { get; set; }
        int StatusCode { get; set; }
    }
    public interface IListResponse<TModel> : IResponse
    {
        IEnumerable<TModel> Data { get; set; }
    }
    public class ListResponse<TModel> : IListResponse<TModel>
    {
        private bool? responseStatus;
        public bool Succeeded
        {
            get => responseStatus ?? (Data?.Any() == true);
            set { responseStatus = value; }
        }
        private readonly List<Errors> _error = new();
        public List<Errors> Errors { get { return _error; } set { } }
        public IEnumerable<TModel> Data { get; set; }
        public string _message;
        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? "" : _message;
            set { _message = value; }
        }
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;

    }

    public class ExtractedObject
    {
        public string ArgName { get; set; }         // e.g. "_set", "objects", "where"
        public ArgRole Role { get; set; }           // Payload, Parameter, or Unknown
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
    public interface ISingleResponse<TModel> : IResponse
    {
        TModel Data { get; set; }
    }
    public class SingleResponse<TModel> : ISingleResponse<TModel>
    {
        private bool? responseStatus;
        public bool Succeeded
        {
            get => responseStatus ?? (Data != null && Convert.ToString(Data) != "" && ((Data.GetType().Name.ToLower() != "boolean") || (Data.ToString().ToLower() != "false")));
            set { responseStatus = value; }
        }
        private readonly List<Errors> _error = new();
        public List<Errors> Errors { get { return _error; } set { } }
        public TModel Data { get; set; }
        public string _message;
        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? "" : _message;
            set { _message = value; }
        }
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    }
}
