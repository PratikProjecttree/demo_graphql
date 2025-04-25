namespace demo_graphql.Models
{
    public class GLRoutingModel
    {
        public string? type { get; set; }
        public string? category { get; set; }
        public string? object_name { get; set; }
        public string? insert_mds_departments { get; set; }
        public string? workflow_meta { get; set; }
        public string? custom_meta { get; set; }
        public Dictionary<string, Field>? input_validation_meta { get; set; }
        // Nested dictionary: entity -> field name -> field definition
        public Dictionary<string, string>? headers { get; set; }
    }

    public class Field
    {
        public string? fieldName { get; set; }
        public bool required { get; set; }
        public string? type { get; set; }
        public int minLength { get; set; }
        public int maxLength { get; set; }
        public string? description { get; set; }
    }
}
