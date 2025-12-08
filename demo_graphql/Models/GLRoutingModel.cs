using System.Text.Json;

namespace demo_graphql.Models
{
    public class GLRoutingModel
    {
        public string? type { get; set; }
        public string? category { get; set; }
        public string? object_name { get; set; }
        public string? inputValidation { get; set; }
        public string? custom_meta { get; set; }
        // public Dictionary<string, Field>? input_validation_meta { get; set; }
        public WorkflowModel? workflow_meta { get; set; }
        // Nested dictionary: entity -> field name -> field definition
        public Dictionary<string, string>? headers { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Dictionary<string, Field>? input_validation_meta => string.IsNullOrWhiteSpace(inputValidation)
        ? null
        : JsonSerializer.Deserialize<Dictionary<string, Field>>(inputValidation, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    }

    public class Field
    {
        public string? fieldName { get; set; }
        public bool required { get; set; }
        public string? type { get; set; }
        public int minLength { get; set; }
        public int maxLength { get; set; }
        public string? description { get; set; }
        public bool isNotAllow { get; set; }
    }
    public class WorkflowModel
    {
        public string? uri { get; set; }
        public Dictionary<string, string>? header { get; set; }
        public string? method { get; set; }
        public Dictionary<string, object>? payload { get; set; }
    }
}
