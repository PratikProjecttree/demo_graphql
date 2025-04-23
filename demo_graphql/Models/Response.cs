namespace demo_graphql.Models
{
    public class Response
    {
        public object? data { get; set; }
        public List<ResponseMessage> responseMessage { get; set; } = new List<ResponseMessage>();


    }

    public class ResponseMessage
    {
        public Guid? id { get; set; }
        public string? type { get; set; }
        public string? message { get; set; }
    }
}
