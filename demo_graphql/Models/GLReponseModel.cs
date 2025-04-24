namespace demo_graphql.Models
{

    public class GLResponseModel
    {
        public bool succeeded { get { return errors == null ? true : false; } }
        public object? data { get; set; }
        public List<GLErrors>? errors { get; set; }
    }

    public class GLErrors
    {
        public string? message { get; set; }
        public GLExtensions? extensions { get; set; }
    }

    public class GLExtensions
    {
        public string? path { get; set; }
        public string? code { get; set; }
    }
}