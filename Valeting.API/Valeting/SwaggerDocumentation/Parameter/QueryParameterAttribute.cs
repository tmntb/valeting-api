namespace Valeting.SwaggerDocumentation.Parameter
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        public string Description { get; set; }
        public string Example { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string Format { get; set; }

        public QueryParameterAttribute(string description, string example, int minimum, int maximum)
        {
            Description = description;
            Example = example;
            Minimum = minimum;
            Maximum = maximum;
        }

        public QueryParameterAttribute(string description, string example, int minimum)
        {
            Description = description;
            Example = example;
            Minimum = minimum;
        }
    }
}
